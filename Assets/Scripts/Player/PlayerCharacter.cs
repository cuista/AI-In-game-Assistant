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
    [SerializeField] private TextMeshProUGUI _countdownText;
    private bool _resetCountdown;

    // Start is called before the first frame update
    void Start()
    {
        health = Managers.Player.health;
        coins = Managers.Inventory.GetItemCount("Coin");
        _countdownText.enabled = false;
        _resetCountdown = false;
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

    public IEnumerator UICountdown(int timer)
    {
        _countdownText.enabled = true;
        for (int i = timer; i > 0 && _countdownText.enabled; i--)
        {
            if(_resetCountdown)
            {
                i = timer;
                _resetCountdown = false;
            }
            _countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        _countdownText.text = "0";
        _countdownText.enabled = false;
    }

    public void ResetUICountdown()
    {
        _resetCountdown = true;
    }

    public void DisableUICountdown()
    {
        _countdownText.enabled = false;
    }

}
