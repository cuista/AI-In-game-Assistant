using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveSentryAI : MonoBehaviour, IEnemy
{
    [SerializeField] public GameObject explosionEffect;
    public float speed = 4.0f;
    public float obstacleRange = 1.0f;
    public float rotationSpeed = 5.0f;
    public float range = 10.0f;
    public const float baseSpeed = 3.0f;

    private Vector3 _defaultPosition;
    private bool _canShoot;
    private bool _isFollowing;
    private bool _isPatrolling;

    private EnemyCharacter _enemyCharacter;

    // Start is called before the first frame update
    void Start()
    {
        _defaultPosition = transform.position;

        _enemyCharacter = GetComponent<EnemyCharacter>();
        SetMoving(true);

        _isPatrolling = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsMoving())
        {
            transform.Translate(0, 0, speed * Time.deltaTime); //move continuosly enemy
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        _canShoot = false;
        _isFollowing = false;
        foreach (var hitCollider in hitColliders)
        {
            GameObject hitOverlapSphere = hitCollider.transform.gameObject;
            // check if player is within range
            if (hitOverlapSphere.CompareTag("Player") || hitOverlapSphere.CompareTag("Clone"))
            {
                _isFollowing = true;
                RaycastHit hitLinecast;
                // if there are NO obstacles between this enemy and player
                if (Physics.Linecast(transform.position, hitOverlapSphere.transform.position, out hitLinecast) && (hitLinecast.transform.gameObject.CompareTag("Player") || hitOverlapSphere.CompareTag("Clone")))
                {
                    Vector3 direction = (hitOverlapSphere.transform.position - transform.position).normalized;
                    Quaternion toRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation

                    if (Vector3.Distance(transform.position, hitOverlapSphere.transform.position) < 3f)
                    {
                        _canShoot = true;
                    }
                }
                break;
            }
        }

        // really close to default position
        if (Vector3.Distance(transform.position, _defaultPosition) < 5f)
        {
            _isPatrolling = true;
        }
        // returning to default position
        if (!_isFollowing && !_isPatrolling)
        {
            Vector3 forwardDir = (_defaultPosition - transform.position).normalized;
            Vector3 rightDir = Quaternion.Euler(0, 90, 0) * forwardDir;
            Vector3 leftDir = Quaternion.Euler(0, -90, 0) * forwardDir;
            RaycastHit forwardHit, rightHit, leftHit;
            if (!Physics.Raycast(transform.position, forwardDir, out forwardHit) || forwardHit.distance > 4f)
            {
                Quaternion toRotation = Quaternion.LookRotation(forwardDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation
            }
            else
            {
                bool hasHittedRight = Physics.Raycast(transform.position, rightDir, out rightHit);
                bool hasHittedLeft = Physics.Raycast(transform.position, leftDir, out leftHit);
                if (!hasHittedRight || (hasHittedLeft && rightHit.distance >= leftHit.distance && rightHit.distance > 4f))
                {
                    Quaternion toRotation = Quaternion.LookRotation(rightDir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation
                }
                else if (!hasHittedLeft || (hasHittedRight && leftHit.distance >= rightHit.distance && leftHit.distance > 4f))
                {
                    Quaternion toRotation = Quaternion.LookRotation(leftDir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                    transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation
                }
                else
                {
                    if (forwardHit.distance >= rightHit.distance && forwardHit.distance >= leftHit.distance)
                    {
                        Quaternion toRotation = Quaternion.LookRotation(forwardDir);
                        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation
                    }
                    else if (rightHit.distance >= forwardHit.distance && rightHit.distance >= leftHit.distance)
                    {
                        Quaternion toRotation = Quaternion.LookRotation(rightDir);
                        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation
                    }
                    else
                    {
                        Quaternion toRotation = Quaternion.LookRotation(leftDir);
                        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0); // only y rotation
                    }
                }

            }
        }

        //avoid walls and check for patrol position or player distance
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.75f, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hit.distance < obstacleRange)
            {
                float patrolAngle = Random.Range(-110, 110);
                transform.Rotate(0, patrolAngle, 0);
            }
        }
        else if (Vector3.Distance(transform.position, _defaultPosition) > range)
        {
            if (_isFollowing)
            {
                transform.Translate(0, 0, -speed * Time.deltaTime);
            }
            else
            {
                _isPatrolling = false; // trigger return to default position
            }
        }

        //Start shooting at player
        if (_canShoot)
        {
            StartCoroutine(Shoot());
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
        speed = baseSpeed * value;
    }

    private IEnumerator Shoot()
    {
        if (!GameEvent.isPaused)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
            foreach (var hitCollider in hitColliders)
            {
                GameObject hitOverlapSphere = hitCollider.transform.gameObject;
                // check if player is within range
                if (hitOverlapSphere.CompareTag("Player") || hitOverlapSphere.CompareTag("Clone") || hitOverlapSphere.CompareTag("Enemy"))
                {
                    hitOverlapSphere.GetComponent<ICharacter>().Hurt(1);
                }
                if (hitOverlapSphere.CompareTag("ReactiveObject"))
                {
                    hitOverlapSphere.GetComponent<IReactiveObject>().ReactToHits(1);
                }
            }

            ExplosionController.MakeItBoom(explosionEffect, transform);
            Destroy(this.gameObject);
        }

        yield return null;
    }
}
