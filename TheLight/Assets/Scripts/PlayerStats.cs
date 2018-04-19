using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour {

    public Text lightCount;
    public Text killCount;

    void Start ()
    {
        if (UserAccountManager.isLoggedIn)
        UserAccountManager.instance.GetData(OnReceivedData);
    }

    void OnReceivedData (string data)
    {
        killCount.text = DataTranslator.DataToKills(data).ToString() + " KILLS";
        lightCount.text = DataTranslator.DataToLight(data).ToString() + " LIGHT";
        Debug.Log(data);
    }
	
}
