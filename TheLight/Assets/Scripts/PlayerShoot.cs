using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof (WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";
    
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    public GameObject ball;
    
    public float speed = 50;

    private Vector3 hitPoint;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;


    void Start()
    {
        if (cam == null)
        {  
            Debug.LogError("PlayerShoot: No Camera referenced");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon(); 

        if(currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        } else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            } else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }

    // Called on server when player shoots
    [Command(channel = 0)]
    void CmdOnShoot()
    {
        
        RpcShootEffect();
    }

    // Called on all clients to do shoot effects
    [ClientRpc]
    void RpcShootEffect()
    {

        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        Vector3 bulletOrigin = weaponManager.GetCurrentGraphics().muzzleFlash.transform.position;
        Vector3 bulletNormal = weaponManager.GetCurrentGraphics().muzzleFlash.transform.rotation.eulerAngles;
        GameObject newBall = (GameObject)Instantiate(ball, bulletOrigin, Quaternion.LookRotation(bulletNormal));
        newBall.GetComponent<Rigidbody>().velocity = (hitPoint - transform.position).normalized * speed;
        //// Creates a bullet particle to run along thr ray trace linne 
        ////GameObject newBall = Instantiate(ball, transform.position, transform.rotation) as GameObject;
        //GameObject newBall = (GameObject)Instantiate(ball, transform.position, transform.rotation);
        //newBall.GetComponent<Rigidbody>().velocity = (hitPoint - transform.position).normalized * speed;
        Destroy(newBall, 10f);
    }

    //Called when somehing is hit and takes hit point and normal of the surface
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcHitEffect(_pos, _normal);
    }

    // Called on all clients to spawn in hit effects
    [ClientRpc]
    void RpcHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        //Destroys the hit effect after stated time to stop the scene cluttering up
        Destroy(_hitEffect, 2f);
    }
    
    


    [Client]
    void Shoot()
    {
        if(!isLocalPlayer)
        {
            return;
        }
       // Debug.Log("shootPart");

        

        RaycastHit _hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            hitPoint = _hit.point;

            // We are shooting, call OnShoot method on server
            CmdOnShoot();

            //newBall.gameObject.GetComponent<BulletProperties>().shooterName = GetComponent<Collider>().gameObject.name;
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }

            //If something is hit call the OnHit method on the server
            CmdOnHit(_hit.point, _hit.normal);
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