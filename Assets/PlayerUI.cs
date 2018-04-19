using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform healthBarFill;

    [SerializeField]
    Text collectedLightAmountText;

    private Player player;
    private PlayerController controller;

    public void SetPlayer (Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        SetHealthAmount(player.GetHealthPct());
        SetCollectedLightAmount(player.GetCollectedLightAmount());
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