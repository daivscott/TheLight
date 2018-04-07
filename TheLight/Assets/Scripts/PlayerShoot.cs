using UnityEngine;
using UnityEngine.Networking;


public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private PlayerWeapon weapon;

    [SerializeField]
    private GameObject weaponGFX;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    public GameObject ball;
    public float speed = 50;

    private Vector3 hitPoint;


    void Start()
    {
        if (cam == null)
        {  
            Debug.LogError("PlayerShoot: No Camera referenced");
            this.enabled = false;
        }
        weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);
    }

    void Update()
    {
        if(weapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        } else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / weapon.fireRate);
            } else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    // Called on server when player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcShootEffect();
    }

    // Called on all clients to do shoot effects
    [ClientRpc]
    void RpcShootEffect()
    {
        //GetComponent<WeaponGraphics>().muzzleFlash.Play();
        GameObject newBall = Instantiate(ball, transform.position, transform.rotation) as GameObject;
        newBall.GetComponent<Rigidbody>().velocity = (hitPoint - transform.position).normalized * speed;
    }



    [Client]
    void Shoot()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        

        RaycastHit _hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, mask))
        {
            hitPoint = _hit.point;
            // We are shooting, call OnShoot method on server
            CmdOnShoot();

            
            //newBall.gameObject.GetComponent<BulletProperties>().shooterName = GetComponent<Collider>().gameObject.name;
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, weapon.damage);
            }
        }
    }

    [Command]
    void CmdPlayerShot (string _playerID, int _damage)
    {
        Debug.Log(_playerID + " has been shot.");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}