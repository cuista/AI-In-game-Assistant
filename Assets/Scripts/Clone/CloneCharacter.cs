using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloneCharacter : MonoBehaviour, ICharacter
{
    private int health;
    [SerializeField] private Slider healthBar;
    private float barValueDamage;

    private CapsuleCollider _capsuleCollider;
    private bool _isHittingGround;

    private Camera _camera;

    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = Managers.Clone.health;
        healthBar.maxValue = Managers.Clone.maxHealth;
        barValueDamage = Managers.Clone.barValueDamage;

        _isHittingGround = true;
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.transform.rotation = Quaternion.LookRotation(healthBar.transform.position - _camera.transform.position);

        if (health <= 0)
        {
            Death();
        }
    }

    public bool IsHittingGround(bool isPlayerHittingGround)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (_capsuleCollider.height + _capsuleCollider.radius) / 1.9f;
            _isHittingGround = hit.distance <= check;
        }
        return _isHittingGround;
    }

    public void Hurt(int damage)
    {
        health -= damage;
        healthBar.value -= barValueDamage * damage;
    }

    public void Death() //FIXME
    {
        gameObject.SetActive(false); //Like this the List in CloningSystem works well!
        //StartCoroutine(Die());
        //Destroy(gameObject); //If I do like this the List in CloningSystem won't work
    }
}
