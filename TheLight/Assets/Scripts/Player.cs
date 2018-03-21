using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

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

    [SerializeField]
    [SyncVar]
    private int currentLightCollected;

    [SerializeField]
    private float healthRegenSpeed = 10f;

    private Vector3 deathLocation;

    //[SyncVar]
    public Transform Spawner;

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }

    public int GetCollectedLightAmount()
    {
        return currentLightCollected;
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    public void Setup()
    {
        // disable components on death
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
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
            RpcTakeDamage(50);
        }

        // Stop the health and health bar going beyond zero
        if(currentHealth < 0f)
        {
            currentHealth = 0f;
        }

        // Regenerate Health
        if (currentHealth < maxHealth && currentHealth > 0f)
        {
            currentHealth += healthRegenSpeed * Time.deltaTime;
        }
        Spawner.GetComponent<CircSpawner>().numObjects = currentLightCollected;
    }

    [ClientRpc]
    public void RpcTakeDamage(float _amount)
    {

        if (_isDead)
        {
            return;
        }
            
        currentHealth -= _amount;

        currentLightCollected += 1;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(currentHealth <= 0)
        {
            deathLocation = this.transform.position;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
        {
            _col.enabled = false;
        }

        Instantiate(Spawner, deathLocation, Quaternion.identity);
        //Spawner.GetComponent<CircSpawner>().numObjects = currentLightCollected;
        Debug.Log(transform.name + " is Dead!");

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name + " respawned");
    }
    
    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        currentLightCollected = 0;

        Spawner.GetComponent<CircSpawner>().numObjects = 0;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if(_col != null)
        {
            _col.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            currentLightCollected = currentLightCollected + 1;
        }
    }
}

