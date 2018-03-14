using UnityEngine;


public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform healthBarFill;

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
    }

    void SetHealthAmount(float _amount)
    {
            healthBarFill.localScale = new Vector3(_amount, 1f, 1f);        
    }

}