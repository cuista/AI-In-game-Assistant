using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterSystem : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    public GameObject bulletCreationPoint;
    public float meleeRadius = 2f;

    private bool shootPressed;
    private bool meleePressed;

    private Animator _animator;

    private AudioSource _audioSource;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip meleeSound;

    // Start is called before the first frame update
    void Start()
    {
        shootPressed = false;
        meleePressed = false;

        _animator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameEvent.isPaused)
        {
            shootPressed = Input.GetButtonDown("Shoot") || shootPressed;
            meleePressed = Input.GetButtonDown("Melee") || meleePressed;      
        }
    }

    private void FixedUpdate()
    {
        if (!GameEvent.isPaused)
        {
            //Shooting attack
            if (shootPressed)
            {
                if(Managers.Inventory.GetItemCount("EnergyRecharge") > 0)
                {
                    Managers.Inventory.ConsumeItem("EnergyRecharge");
                    GameObject bullet = Instantiate(bulletPrefab) as GameObject;
                    bullet.transform.position = (bulletCreationPoint != null) ? bulletCreationPoint.transform.position : transform.TransformPoint(Vector3.forward * 2.5f);
                    bullet.transform.rotation = bulletCreationPoint.transform.rotation;

                    _animator.SetTrigger("Shoot");
                    _audioSource.PlayOneShot(shootSound);
                }
                shootPressed = false; //Reset shoot input
            }

            //Melee attack
            if(meleePressed)
            {
                Vector3 meleePoint = transform.position + transform.forward * 0.8f;
                Collider[] hitColliders = Physics.OverlapSphere(meleePoint, meleeRadius);
                foreach (var hitCollider in hitColliders)
                {

                    GameObject hitObject = hitCollider.transform.gameObject;
                    IReactiveObject target = hitObject.GetComponent<IReactiveObject>();
                    if (target != null)
                    {
                        target.ReactToHits(1);
                        target.AddHitForce(2, transform.position, meleeRadius);
                    }
                }
                meleePressed = false; //Reset melee input

                switch ((int) Random.Range(0, 3))
                {
                    case 0:_animator.SetTrigger("Melee1");break;
                    case 1: _animator.SetTrigger("Melee2"); break;
                    case 2: default: _animator.SetTrigger("Melee3"); break;
                }
                _audioSource.PlayOneShot(meleeSound);
            }
        }
    }

    public GameObject GetBulletPrefab()
    {
        return bulletPrefab;
    }
}
