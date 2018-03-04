using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    [SyncVar]
    private int currrentHealth;

    void Awake()
    {
        SetDefaults();
    }

    public void TakeDamage(int _amount)
    {
        currrentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currrentHealth + " health.");
    }
    
    public void SetDefaults()
    {
        currrentHealth = maxHealth;        
    }
}

