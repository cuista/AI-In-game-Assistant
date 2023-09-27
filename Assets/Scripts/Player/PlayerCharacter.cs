using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    private int health;
    private int coins;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _coinsText;

    // Start is called before the first frame update
    void Start()
    {
        health = Managers.Player.health;
        coins = Managers.Inventory.GetItemCount("Coin");
    }

    // Update is called once per frame
    void Update()
    {
        health = Managers.Player.health;
        coins = Managers.Inventory.GetItemCount("Coin");
        _healthText.text = health.ToString();
        _coinsText.text = coins.ToString();
        if (health <= 0)
        {
            Death();
        }
    }
    public void Death()
    {
        //StartCoroutine(Die());
    }

}
