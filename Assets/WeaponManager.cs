using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;
    
    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    void Start()
    {
        EquipWeapon(primaryWeapon); 
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        GameObject _weaponInstance = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponInstance.transform.SetParent(weaponHolder);

        currentGraphics = _weaponInstance.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
            Debug.LogError("No WeaponGraphics component on the weapon object. " + _weaponInstance.name);


        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponInstance, LayerMask.NameToLayer(weaponLayerName));

    }

}
