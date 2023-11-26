using Google.Api;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour, ICharacter
{
    private int health;
    private int healthKitA;
    private int healthKitB;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image fillHealthBar;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private Image damageImage;
    private Color flashColor = new Color(1f, 0f, 0f, 0.7f);
    private float flashSpeed = 5f;
    private float barValueDamage;
    private Image healthBarBackground;
    private bool damaged;
    private int coins;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private TextMeshProUGUI _energyRechargesText;
    [SerializeField] private TextMeshProUGUI _cloneRechargesText;
    [SerializeField] private TextMeshProUGUI _countdownText;
    private bool _resetCountdown;
    [SerializeField] private Image _switchedCloneMode;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.SWITCHED_CLONE_MODE, OnSwitchedCloneMode);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.SWITCHED_CLONE_MODE, OnSwitchedCloneMode);
    }

    // Start is called before the first frame update
    void Start()
    {
        health = Managers.Player.health;
        healthBar.maxValue = Managers.Player.maxHealth;
        healthKitA = Managers.Player.healthkitA;
        healthKitB = Managers.Player.healthkitB;
        barValueDamage = Managers.Player.barValueDamage;
        healthBarBackground = healthBar.GetComponentInChildren<Image>();
        coins = Managers.Inventory.GetItemCount("Coin");

        gameOver.SetActive(false);

        _countdownText.enabled = false;
        _resetCountdown = false;

        _switchedCloneMode.color = Color.red;

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        coins = Managers.Inventory.GetItemCount("Coin");
        _coinsText.text = coins.ToString();
        _energyRechargesText.text = Managers.Inventory.GetItemCount("EnergyRecharge").ToString();
        _cloneRechargesText.text = Managers.Inventory.GetItemCount("CloneRecharge").ToString();

        if (!GameEvent.isPaused)
        {
            //restore 10% health
            if (Managers.Inventory.GetItemCount("HealthkitA") > 0)
            {
                health += healthKitA;
                healthBar.value += (barValueDamage * healthKitA);

                if (health > Managers.Player.health)
                {
                    health = Managers.Player.health;
                    healthBar.value = healthBar.maxValue;
                }

                Managers.Inventory.ConsumeItem("HealthkitA");
            }
            //restore 50% health
            else if (Managers.Inventory.GetItemCount("HealthkitB") > 0)
            {
                health += healthKitB;
                healthBar.value += (barValueDamage * healthKitB);

                if (health > Managers.Player.health)
                {
                    health = Managers.Player.health;
                    healthBar.value = healthBar.maxValue;
                }

                Managers.Inventory.ConsumeItem("HealthkitB");
            }

            if (health <= 0)
            {
                Death();
            }
            if (damaged)
            {
                damageImage.color = flashColor;
            }
            else
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }
            damaged = false;
        }
    }

    //player lose one or more lives (damage)
    public void Hurt(int damage)
    {
        damaged = true;
        health -= damage;
        healthBar.value -= barValueDamage * damage;
        _audioSource.PlayOneShot(hurtSound);
    }

    //Trigger the gameOver
    public void Death()
    {
        fillHealthBar.enabled = false;
        gameOver.SetActive(true);
        //gameOver.transform.GetChild(1).gameObject.SetActive(false);
        healthBarBackground.color = Color.red;
        Messenger.Broadcast(GameEvent.GAMEOVER);

        GameEvent.isPaused = true;
        _audioSource.PlayOneShot(deathSound);
        //StartCoroutine(Die());

        /* FIXME MOMENTANEO */
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0; // stop everything (PAUSE)
        /* */
    }

    private IEnumerator Die()
    {
        //GetComponent<Animator>().SetBool("Dying", true);

        Image gameOverLogo = gameOver.GetComponentInChildren<Image>();
        Vector3 finalPosition = gameOverLogo.transform.position;
        Color color = gameOverLogo.color;
        color.a = 0;
        gameOverLogo.color = color;

        float duration = 5f;
        float totalTime = 0;
        while (totalTime <= duration)
        {
            color.a = totalTime / duration;
            gameOverLogo.transform.position = new Vector3(finalPosition.x, finalPosition.y - (1 - (totalTime / duration)) * 500, finalPosition.z);
            gameOverLogo.color = color;
            totalTime += Time.deltaTime;
            yield return null;
        }

        //gameOver.transform.GetChild(1).gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0; // stop everything (PAUSE)
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

    private void OnSwitchedCloneMode()
    {
        _switchedCloneMode.color = _switchedCloneMode.color == Color.red ? new Color(1.0f, 0.64f, 0.0f) : Color.red;
    }

}
