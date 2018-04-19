﻿using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform healthBarFill;

    [SerializeField]
    Text collectedLightAmountText;

    [SerializeField]
    GameObject pauseMenu;

    private Player player;
    private PlayerController controller;

    public void SetPlayer (Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
    }

    void Start()
    {
        PauseMenu.isOn = false;
    }

    void Update()
    {
        SetHealthAmount(player.GetHealthPct());
        SetCollectedLightAmount(player.GetCollectedLightAmount());

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    void SetHealthAmount(float _amount)
    {
            healthBarFill.localScale = new Vector3(_amount, 1f, 1f);        
    }

    void SetCollectedLightAmount(int _amount)
    {
        collectedLightAmountText.text = _amount.ToString();
    }

}