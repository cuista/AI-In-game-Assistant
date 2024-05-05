using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour, ICharacter
{
    private int health;
    private int PotionBig;
    private int PotionSmall;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image fillHealthBar;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private Image damageImage;
    private Color flashColor = new Color(1f, 0f, 0f, 0.7f);
    private float flashSpeed = 5f;
    private float barValueDamage;
    private Image healthBarBackground;
    private bool damaged;
    private int gems;
    [SerializeField] private TextMeshProUGUI _gemsText;
    [SerializeField] private TextMeshProUGUI _gemsTotalText;
    [SerializeField] private GameObject _cloneRechargesIcons;
    private bool _spawnPointPlaced = false;
    [SerializeField] private TextMeshProUGUI _countdownText;
    private bool _resetCountdown;
    private Animator _spawnCloneAnimator;
    [SerializeField] private Image _switchedCloneMode;

    [SerializeField] public GameObject hitEffect;

    private Animator _animator;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.SWITCHED_CLONE_MODE, OnSwitchedCloneMode);
        Messenger.AddListener(GameEvent.SPAWN_POINT_PLACED, OnSpawnPointPlaced);
        Messenger.AddListener(GameEvent.SPAWN_POINT_EXPIRED, OnSpawnPointExpired);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.SWITCHED_CLONE_MODE, OnSwitchedCloneMode);
        Messenger.RemoveListener(GameEvent.SPAWN_POINT_PLACED, OnSpawnPointPlaced);
        Messenger.RemoveListener(GameEvent.SPAWN_POINT_EXPIRED, OnSpawnPointExpired);
    }

    // Start is called before the first frame update
    void Start()
    {
        health = Managers.Player.health;
        healthBar.maxValue = Managers.Player.maxHealth;
        PotionBig = Managers.Player.PotionBig;
        PotionSmall = Managers.Player.PotionSmall;
        barValueDamage = Managers.Player.barValueDamage;
        healthBarBackground = healthBar.GetComponentInChildren<Image>();
        gems = Managers.Inventory.GetItemCount("Gems");

        for (int i = 0; i < _cloneRechargesIcons.transform.childCount; i++)
        {
            Transform iconTransform = _cloneRechargesIcons.transform.GetChild(i);
            iconTransform.gameObject.SetActive(false);
            iconTransform.GetChild(0).gameObject.SetActive(false);
        }

        gameOver.SetActive(false);

        _countdownText.enabled = false;
        _resetCountdown = false;

        _switchedCloneMode.color = Color.red;

        hitEffect.transform.localScale.Set(2f, 2f, 2f);

        _animator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        gems = Managers.Inventory.GetItemCount("Gems");
        _gemsText.text = gems.ToString();

        for (int i = 0; i <_cloneRechargesIcons.transform.childCount; i++)
        {
            Transform iconTransform = _cloneRechargesIcons.transform.GetChild(i);

            if(i < Managers.Inventory.GetItemCount("CloneRecharge"))
            {
                iconTransform.gameObject.SetActive(true);
                if(_spawnPointPlaced && i == Managers.Inventory.GetItemCount("CloneRecharge") - 1)
                {
                    iconTransform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    iconTransform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
            {
                iconTransform.gameObject.SetActive(false);
                iconTransform.GetChild(0).gameObject.SetActive(false);
            }
        }

        if (!GameEvent.isPaused)
        {
            //restore 10% health
            if (Managers.Inventory.GetItemCount("PotionBig") > 0)
            {
                health += PotionBig;
                healthBar.value += (barValueDamage * PotionBig);

                if (health > Managers.Player.health)
                {
                    health = Managers.Player.health;
                    healthBar.value = healthBar.maxValue;
                }

                Managers.Inventory.ConsumeItem("PotionBig");
            }
            //restore 50% health
            else if (Managers.Inventory.GetItemCount("PotionSmall") > 0)
            {
                health += PotionSmall;
                healthBar.value += (barValueDamage * PotionSmall);

                if (health > Managers.Player.health)
                {
                    health = Managers.Player.health;
                    healthBar.value = healthBar.maxValue;
                }

                Managers.Inventory.ConsumeItem("PotionSmall");
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
        ExplosionController.MakeItBoom(hitEffect, transform);
    }

    //Trigger the gameOver
    public void Death()
    {
        fillHealthBar.enabled = false;
        gameOver.SetActive(true);
        gameOver.transform.GetChild(1).gameObject.SetActive(false);
        healthBarBackground.color = Color.red;
        Messenger.Broadcast(GameEvent.GAMEOVER);

        GameEvent.isPaused = true;
        _audioSource.PlayOneShot(deathSound);
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        _animator.SetBool("Dying", true);

        DontDestroyOnLoadManager.GetAudioManager().PlaySoundtrackGameOver();

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
            totalTime += Time.fixedDeltaTime;
            yield return null;
        }

        gameOver.transform.GetChild(1).gameObject.SetActive(true);

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
            if(i == 5 && _spawnCloneAnimator != null)
            {
                _spawnCloneAnimator.SetTrigger("Flash");
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

    public void SetSpawnCloneAnimator(Animator spawnCloneAnimator)
    {
        _spawnCloneAnimator = spawnCloneAnimator;
    }

    public void DisableUICountdown()
    {
        _countdownText.enabled = false;
    }

    private void OnSwitchedCloneMode()
    {
        _switchedCloneMode.color = _switchedCloneMode.color == Color.red ? new Color(1.0f, 0.64f, 0.0f) : Color.red;
    }

    private void OnSpawnPointPlaced()
    {
        _spawnPointPlaced = true;
    }

    private void OnSpawnPointExpired()
    {
        _spawnPointPlaced = false;
    }

    public void SetGemsTotal(int total)
    {
        if(total >= 0)
        {
            _gemsTotalText.text = total.ToString();
        }
        else
        {
            _gemsTotalText.text = "\u221E";
        }
    }    
}
