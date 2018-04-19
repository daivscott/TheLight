using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    Player player;

	void Start ()
    {
        player = GetComponent<Player>();
        StartCoroutine(SyncScoreLoop());
	}

    void OnDestroy()
    {
        if (player != null)
            SyncNow();
    }
	
    IEnumerator SyncScoreLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);

            SyncNow();
        }          
    }

    void SyncNow()
    {
        if (UserAccountManager.isLoggedIn)
        {
            UserAccountManager.instance.GetData(OnDataRecieved);
        }
    }

    void OnDataRecieved(string data)
    {
        if (player.kills == 0 && player.lightStored == 0)
        {
            return;
        }

        int kills = DataTranslator.DataToKills(data);
        int light = DataTranslator.DataToLight(data);

        int newKills = player.kills + kills;
        int newlight = player.lightStored + light;

        string newData = DataTranslator.ValuesToData(newKills, newlight);

        Debug.Log("Syncing: " + newData);

        player.kills = 0;
        player.lightStored = 0;

        UserAccountManager.instance.SendData(newData);

    }
}
