using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;
    public float rotationSpeed = 3.0f;
    public float range = 30.0f;
    public float fireDelay = 1f;

    private Quaternion _defaultRotation;
    private float _shootTimer;
    private bool _canShoot;

    private EnemyCharacter _enemyCharacter;

    public GameObject bulletCreationPoint;

    // Start is called before the first frame update
    void Start()
    {
        _defaultRotation = transform.rotation;
        _shootTimer = 0;

        _enemyCharacter = GetComponent<EnemyCharacter>();
        SetMoving(true);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (!GameEvent.isPaused)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
            _canShoot = false;
            bool playerHitted = false;
            GameObject closestCloneHitted = null;
            float closestCloneDistance = range;
            foreach (var hitCollider in hitColliders)
            {
                GameObject hitOverlapSphere = hitCollider.transform.gameObject;
                // check if player is within range
                if (hitOverlapSphere.CompareTag("Player"))
                {
                    playerHitted = true;
                    RaycastHit hitLinecast;
                    // if there are NO obstacles between turret and player
                    if (Physics.Linecast(transform.position, hitOverlapSphere.transform.position, out hitLinecast) && hitLinecast.transform.gameObject.CompareTag("Player"))
                    {
                        if (IsMoving())
                        {
                            Vector3 direction = (hitOverlapSphere.transform.position - transform.position).normalized;
                            Quaternion toRotation = Quaternion.LookRotation(direction);
                            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
                            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation
                        }
                        _canShoot = true;
                    }
                }
                else if (hitOverlapSphere.CompareTag("Clone")) //Update the closest clone
                {
                    float currentDistance = Vector3.Distance(hitOverlapSphere.transform.position, transform.position);
                    if (closestCloneHitted == null || currentDistance < closestCloneDistance)
                    {
                        closestCloneHitted = hitOverlapSphere;
                        closestCloneDistance = currentDistance;
                    }
                }
            }

            if(!playerHitted && closestCloneHitted != null) //If there's no player in the range, turret shots the closest clone!
            {
                RaycastHit hitLinecast;
                // if there are NO obstacles between turret and player
                if (Physics.Linecast(transform.position, closestCloneHitted.transform.position, out hitLinecast) && hitLinecast.transform.gameObject.CompareTag("Clone"))
                {
                    if (IsMoving())
                    {
                        Vector3 direction = (closestCloneHitted.transform.position - transform.position).normalized;
                        Quaternion toRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime);
                        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation
                    }
                    _canShoot = true;
                }
            }

            //start shoot the player
            if (_canShoot)
            {
                _shootTimer += Time.fixedDeltaTime;
                if (_shootTimer > fireDelay)
                {
                    if (!GameEvent.isPaused)
                    {
                        GameObject bullet = Instantiate(bulletPrefab) as GameObject;
                        bullet.transform.position = (bulletCreationPoint != null) ? bulletCreationPoint.transform.position : transform.TransformPoint(Vector3.forward * 2.5f);
                        bullet.transform.rotation = transform.rotation;
                        _shootTimer = 0;
                    }
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _defaultRotation, rotationSpeed * Time.fixedDeltaTime);
                _shootTimer = 0;
            }
        }
    }

    public void RemoveLives(int livesToRemove)
    {
        _enemyCharacter.RemoveLives(livesToRemove);
    }

    public int GetLives()
    {
        return _enemyCharacter.GetLives();
    }

    public bool IsMoving()
    {
        return _enemyCharacter.IsMoving();
    }

    public void SetMoving(bool moving)
    {
        _enemyCharacter.SetMoving(moving);
    }

    public void OnSpeedChanged(float value)
    {

    }
}

