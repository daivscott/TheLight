﻿using UnityEngine;
using UnityEngine.UI;

public class UserAccount_Lobby : MonoBehaviour {

    public Text usernameText;

    void Start()
    {
        if(UserAccountManager.isLoggedIn)
            usernameText.text = "Logged In As: " + UserAccountManager.playerUsername;
    }

    public void LogOut()
    {
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.LogOut(); 
    }

}
