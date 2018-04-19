using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    public AudioSource audioSource;
    public AudioClip pickupCollected;

    public AudioSource audioSource2;
    public AudioClip uploadLightClip;

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private float maxHealth = 100f;

    [SerializeField]
    [SyncVar]
    private float currentHealth;

    // Kills and light that are synced to the network
    public int kills;
    public int lightStored;

    // Kills and light that are stored in the session
    public int sessionKills;
    public int sessionStoredLight;

    [SerializeField]
    public int currentLightCollected;

    [SerializeField]
    private float healthRegenSpeed = 10f;

    [SyncVar]
    private Vector3 deathLocation;

    [SerializeField]
    private int numObjects;

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    private bool[] wasEnabled;

    public GameObject lightPrefab;

    //public Animator anim;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource2 = GetComponent<AudioSource>();
    }

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }


    public int GetCollectedLightAmount()
    {
        return currentLightCollected;
    }

    public int GetSessionKillsAmount()
    {
        return sessionKills;
    }
    public int GetSessionStoredLightAmount()
    {
        return sessionStoredLight;
    }



    public void SetupPlayer()
    {
        if(isLocalPlayer)
        {
            // Switch cameras for respawn
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        
        CmdBroadcastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
            // disable components on death
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }
        
        // Reset settings to default
        SetDefaults();
    }

    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        // Test function to cause damage
        if(Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(50, transform.name);
        }

        // Stop the health and health bar going beyond zero
        if(currentHealth < 0f)
        {
            currentHealth = 0f;
        }

        CmdRegenerateHealth();

        CmdSetNumberOfObjectsToSpawn();
    }



    

    [Command(channel = 0)]
    void CmdSetNumberOfObjectsToSpawn()
    {
        numObjects = currentLightCollected;
    }

    [Command]
    void CmdRegenerateHealth()
    {
        if (currentHealth < maxHealth && currentHealth > 0f)
        {
            currentHealth += healthRegenSpeed * Time.deltaTime;
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(float _amount, string _sourceID)
    {

        if (_isDead)
        {
            return;
        }
            
        currentHealth -= _amount;

        // update collected light on being shot
        currentLightCollected = currentLightCollected + 1;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(currentHealth <= 0)
        {
             
            deathLocation = this.transform.position;
            Die(_sourceID);
        }
    }

    private void Die(string _sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);

        if (sourcePlayer != null)
        {
            sourcePlayer.kills++;
            sourcePlayer.sessionKills++;
        }

        //anim.SetTrigger("Die");

        // Disable components on death
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // Disable GameObjects on death
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        // Disable the colllider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        // Spawn death effect
        GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f); 

        // Switch cameras for death
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }
        
        // Spawn collected light particles from dead player
        SpawnLight();
        
        Debug.Log(transform.name + " is Dead!");

        StartCoroutine(Respawn());
    }


    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned");
    }
    
    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        currentLightCollected = 0;

        numObjects = 0;

        // Enable components on respawn
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // Enable GameObjects on respawn
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // Enable collider  on respawn
        Collider _col = GetComponent<Collider>();
        if(_col != null)
        {
            _col.enabled = true;
        }


        // Create Spawn Effect
        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            //other.gameObject.SetActive(false);
            Destroy(other.gameObject);

            audioSource.PlayOneShot(pickupCollected, 0.7f);

            // update collected light on collecting pickup 
            currentLightCollected = currentLightCollected + 1;
        }
        if(other.gameObject.CompareTag("UploadCube"))
        {
            if (currentLightCollected != 0)
            {
                audioSource2.PlayOneShot(pickupCollected, 0.7f);
                lightStored = lightStored + currentLightCollected;
                sessionStoredLight = sessionStoredLight + currentLightCollected;
            }
            currentLightCollected = 0;
        }
    }

    
    void SpawnLight()
    {
        Vector3[] spawnPositions =  new Vector3[numObjects];
        //Vector3 center = transform.position;
        Vector3 center = deathLocation;
        for (int i = 0; i < numObjects; i++)
        {

            Vector3 pos = RandomCircle(center, 2f);
            
            spawnPositions[i] = new Vector3  (pos.x, pos.y - 0.5f, pos.z);

            GameObject spawnedLightPickup = Instantiate(lightPrefab, spawnPositions[i], Quaternion.identity);
            NetworkServer.Spawn(spawnedLightPickup);

        }
    }


    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
}

