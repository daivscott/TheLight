
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering; //for skinned mesh render options

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    [SerializeField]
    private SkinnedMeshRenderer skinned; // cast shadows only for skinned mesh
    [SerializeField]
    private SkinnedMeshRenderer skinned2; // cast shadows only for skinned mesh

    Camera sceneCamera;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            // Configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("No PlayerUI component on PlayerUI prefab");
            ui.SetPlayer(GetComponent<Player>());

            skinned.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            skinned2.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

        }

        GetComponent<Player>().Setup();
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
    }

    public override void PreStartClient()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
    }

    //public override void OnStartLocalPlayer()
    //{
    //    base.OnStartLocalPlayer();
    //    this.tag = "Player";
    //    this.GetComponent<Collider>().tag = "Player";
    //    this.GetComponent<Player>().tag = "Player";
    //    GetComponent<MeshRenderer>().material.color = Color.blue;
    //}

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents ()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void OnDisable ()
    {
        Destroy(playerUIInstance);

        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }
}
