using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

class PlayerRecord
{
    public readonly Transform playerCamera;
    public readonly bool isFirstPersonView;
    public readonly float headRotationX;
    public readonly float mouseX, horInput, verInput;
    public readonly float moveSpeed;
    public readonly bool jumpPressed;
    public readonly bool shootPressed;
    public readonly bool meleePressed;

    public PlayerRecord(Transform playerCamera, bool isFirstPersonView, float headRotationX, float mouseX, float horInput, float verInput, float moveSpeed, bool jumpPressed, bool shootPressed, bool meleePressed)
    {
        this.playerCamera = playerCamera;
        this.isFirstPersonView = isFirstPersonView;
        this.headRotationX = headRotationX;
        this.mouseX = mouseX;
        this.horInput = horInput;
        this.verInput = verInput;
        this.moveSpeed = moveSpeed;
        this.jumpPressed = jumpPressed;
        this.shootPressed = shootPressed;
        this.meleePressed = meleePressed;
    }
}

class ReplayClone
{
    public GameObject Clone;
    public List<PlayerRecord> PlayerRecordings;
    public bool isJumping;
    public float vertSpeed;
    public GameObject bulletCreationPoint;
    public GameObject head;

    public Animator animator;

    public ReplayClone(GameObject clone, List<PlayerRecord> playerRecordings, float vertSpeed)
    {
        this.Clone = clone;
        this.PlayerRecordings = playerRecordings;
        isJumping = false;
        this.vertSpeed = vertSpeed;
        bulletCreationPoint = clone.GetComponent<CloneCharacter>().GetBulletCreationPoint();
        head = clone.GetComponentInChildren<CloneHead>().gameObject;

        animator = clone.GetComponent<Animator>();
    }

    public ReplayClone(GameObject clone, float vertSpeed)
    {
        this.Clone = clone;
        isJumping = false;
        this.vertSpeed = vertSpeed;
        bulletCreationPoint = clone.GetComponent<CloneCharacter>().GetBulletCreationPoint();
        head = clone.GetComponentInChildren<CloneHead>().gameObject;

        animator = clone.GetComponent<Animator>();
    }
}

public class CloningSystem : MonoBehaviour
{
    [SerializeField] private GameObject lateClonePrefab;
    [SerializeField] private GameObject mirrorClonePrefab;
    [SerializeField] private GameObject spawnClonePrefab;

    private RelativeMovement relativeMovement;
    private FirstPersonLook firstPersonLook;
    private ShooterSystem shooterSystem;

    private List<ReplayClone> _lateClones;
    private List<ReplayClone> _mirrorClones;
    private GameObject _spawnPoint;
    private bool _spawnPointTimedOut;
    private List<PlayerRecord> _currentPlayerRecording;

    private bool fire2Pressed;
    private bool fire1Pressed;

    public float timeAcceleration = 3f;

    private bool _isLateCloneMode;

    float mouseX;
    float horInput;
    float verInput;
    float moveSpeed;
    bool jumpPressed;
    bool shootPressed;
    bool meleePressed;   
    
    private Transform _playerCamera;
    private bool _isFirstPersonView;

    private float _headRotationX;

    [SerializeField] private Volume _VHSEffect;

    private Animator _animator;

    private AudioSource _audioSource;

    [SerializeField] private AudioClip plantSpawnSound;
    [SerializeField] private AudioClip cloneSpawnSound;

    // Start is called before the first frame update
    void Start()
    {
        relativeMovement = GetComponent<RelativeMovement>();
        firstPersonLook = GetComponent<FirstPersonLook>();
        shooterSystem = GetComponent<ShooterSystem>();

        _lateClones = new List<ReplayClone>();
        _mirrorClones = new List<ReplayClone>();
        _spawnPoint = null;
        _spawnPointTimedOut = false;
        _currentPlayerRecording = null;

        fire2Pressed = false;
        fire1Pressed = false;

        _isLateCloneMode = true;
        jumpPressed = false;
        shootPressed = false;
        meleePressed = false;

        _playerCamera = relativeMovement.PlayerCamera;
        _isFirstPersonView = relativeMovement.IsFirstPersonView;

        _headRotationX = firstPersonLook.headRotationX;

        _VHSEffect.enabled = false;

        _animator = GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameEvent.isPaused)
        {
            if (Managers.Inventory.GetItemCount("CloneRecharge") > 0)
            {
                if(_spawnPointTimedOut) //SpawnPoint unused for 10 seconds
                {
                    Destroy(_spawnPoint);
                    _currentPlayerRecording = null;
                    _spawnPointTimedOut = false;
                }

                fire1Pressed = (Input.GetButtonUp("Fire1") && _spawnPoint != null) || fire1Pressed;
                fire2Pressed = Input.GetButtonUp("Fire2") || fire2Pressed;
            }

            //Time acceleration ability
            if (Input.GetButtonDown("TimeAcceleration"))
            {
                Time.timeScale = timeAcceleration;
                _VHSEffect.enabled = true;
            }
            if (Input.GetButtonUp("TimeAcceleration"))
            {
                Time.timeScale = 1.0f;
                _VHSEffect.enabled = false;
            }

            //Switch late and mirror clone
            if(Input.GetButtonUp("SwitchCloneMode"))
            {
                _isLateCloneMode = !_isLateCloneMode;
                Messenger.Broadcast(GameEvent.SWITCHED_CLONE_MODE);
            }

            //For input recording
            mouseX = Input.GetAxis("Mouse X");
            horInput = Input.GetAxis("Horizontal");
            verInput = Input.GetAxis("Vertical");
            moveSpeed = Input.GetButton("Run") ? relativeMovement.runSpeed: relativeMovement.walkSpeed;
            jumpPressed = Input.GetButtonDown("Jump") || jumpPressed;
            shootPressed = Input.GetButtonDown("Shoot") && Managers.Inventory.GetItemCount("EnergyRecharge") > 0 || shootPressed;
            meleePressed = Input.GetButtonDown("Melee") || meleePressed;
        }
    }

    private void FixedUpdate()
    {
        if (!GameEvent.isPaused)
        {
            if (fire2Pressed) //Place clone spawn point
            {
                if(_spawnPoint != null)
                {
                    Destroy(_spawnPoint);
                    _spawnPointTimedOut = false;
                    transform.GetComponent<PlayerCharacter>().ResetUICountdown(); //UI countdown reset
                }
                else
                {
                    StartCoroutine(transform.GetComponent<PlayerCharacter>().UICountdown(20)); //UI countdown of 20sec
                }
                _currentPlayerRecording = new List<PlayerRecord>();
                _spawnPoint = Instantiate(spawnClonePrefab) as GameObject;
                _spawnPoint.transform.position = new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z);
                transform.GetComponent<PlayerCharacter>().SetSpawnCloneAnimator(_spawnPoint.GetComponent<Animator>()); //Flash spawnClone Animation
                StartCoroutine(SpawnPointUnused(_spawnPoint));

                fire2Pressed = false;

                _animator.SetTrigger("PlantSpawn");
                _audioSource.PlayOneShot(plantSpawnSound);
            }

            if (fire1Pressed) //Spawn the clone and starting replaying player movements
            {
                Managers.Inventory.ConsumeItem("CloneRecharge");

                Transform spawn = _spawnPoint.transform;
                Destroy(_spawnPoint);
                if (_isLateCloneMode)
                {
                    GameObject clone = Instantiate(lateClonePrefab, spawn.position, spawn.rotation);
                    List<PlayerRecord> records = _currentPlayerRecording;
                    _lateClones.Add(new ReplayClone(clone, records, relativeMovement.minFall));
                }
                else
                {
                    GameObject clone = Instantiate(mirrorClonePrefab, spawn.position, spawn.rotation);
                    _mirrorClones.Add(new ReplayClone(clone, relativeMovement.minFall));
                }
                _currentPlayerRecording = null;

                transform.GetComponent<PlayerCharacter>().DisableUICountdown();

                fire1Pressed = false;
                _animator.SetTrigger("SpawnClone");
                _audioSource.PlayOneShot(cloneSpawnSound);
            }

            //Update camera info from relativeMovement
            _playerCamera = relativeMovement.PlayerCamera;
            _isFirstPersonView = relativeMovement.IsFirstPersonView;

            //Update look rotation info from firstPersonCamera
            _headRotationX = firstPersonLook.headRotationX;

            //If there's a clone ready to be placed, a not null clone ready that is recording player movements
            _currentPlayerRecording?.Add(new PlayerRecord(_playerCamera, _isFirstPersonView, _headRotationX, mouseX, horInput, verInput, moveSpeed, jumpPressed, shootPressed, meleePressed));

            if (_lateClones.Count > 0) //If there's at least a late clone in the scene
            {
                bool containsDestroyedClones = false;
                foreach (var replayClone in _lateClones)
                {
                    if(replayClone != null)
                    {
                        //ADD player record
                        replayClone.PlayerRecordings.Add(new PlayerRecord(_playerCamera, _isFirstPersonView, _headRotationX, mouseX, horInput, verInput, moveSpeed, jumpPressed, shootPressed, meleePressed));

                        //Clone replays recorded player movements
                        CloneMovement(replayClone, true);

                        //Clone replays recorded player attacks
                        CloneAttack(replayClone, true);

                        //REMOVE player record
                        replayClone.PlayerRecordings.RemoveAt(0);
                    }
                    else
                    {
                        containsDestroyedClones = true;
                    }
                }

                if (containsDestroyedClones)
                {
                    _mirrorClones.RemoveAll(x => x == null);
                }
            }

            if (_mirrorClones.Count > 0) //If there's at least a mirror clone in the scene
            {
                bool containsDestroyedClones = false;
                foreach (var replayClone in _mirrorClones)
                {
                    if(replayClone != null)
                    {
                        //Clone replays recorded player movements
                        CloneMovement(replayClone, false);

                        //Clone replays recorded player attacks
                        CloneAttack(replayClone, false);
                    }
                    else
                    {
                        containsDestroyedClones = true;
                    }
                }

                if(containsDestroyedClones)
                {
                    _mirrorClones.RemoveAll(x => x == null);
                }
            }

            jumpPressed = false; //Reset jump input rec
            shootPressed = false; //Reset shoot input rec
            meleePressed = false; //Reset melee input rec

        }
    }

    private IEnumerator SpawnPointUnused(GameObject spawn) //if a spawn point is unused it will be remove after x seconds
    {
        yield return new WaitForSeconds(20);

        if (spawn != null)
        {
            _spawnPointTimedOut = true;
        }
    }

    void CloneMovement(ReplayClone replayClone, bool isLateClone)
    {
        Transform playerCamera = isLateClone ? replayClone.PlayerRecordings[0].playerCamera : _playerCamera;
        bool isFirstPersonView = isLateClone ? replayClone.PlayerRecordings[0].isFirstPersonView : _isFirstPersonView;
        float headRotX = isLateClone ? replayClone.PlayerRecordings[0].headRotationX : _headRotationX;
        float rotX = isLateClone ? replayClone.PlayerRecordings[0].mouseX : mouseX;
        float hor = isLateClone ? replayClone.PlayerRecordings[0].horInput : horInput;
        float ver = isLateClone ? replayClone.PlayerRecordings[0].verInput : verInput;
        float horSpeed = isLateClone ? replayClone.PlayerRecordings[0].moveSpeed : moveSpeed;
        bool doJump = isLateClone ? replayClone.PlayerRecordings[0].jumpPressed : jumpPressed;

        //Clone HORIZONTAL-MOVEMENT
        Vector3 moveDirection = new Vector3(hor * horSpeed, 0, ver * horSpeed); //(x,0,z)
        moveDirection = Vector3.ClampMagnitude(moveDirection, horSpeed); //avoid diagonal speed-up

        //Handle rotation
        if (moveDirection != Vector3.zero)
        {
            Transform camera = playerCamera;
            camera.eulerAngles = new Vector3(0, camera.eulerAngles.y, 0);
            moveDirection = camera.TransformDirection(moveDirection);

            if (!isFirstPersonView)
            {
                Quaternion direction = Quaternion.LookRotation(moveDirection);
                replayClone.Clone.transform.rotation = Quaternion.Lerp(replayClone.Clone.transform.rotation, direction, 15f * Time.deltaTime); //change rotation smoothly
            }
        }
        if(isFirstPersonView)
        {
            replayClone.Clone.transform.Rotate(0, rotX * firstPersonLook.sensitivityHor, 0);
            replayClone.head.transform.localEulerAngles = new Vector3(headRotX, 0, 0);
        }

        replayClone.animator.SetFloat("Speed", moveDirection.magnitude);

        //Clone VERTICAL-MOVEMENT
        bool isCloneHittingGround;
        if (replayClone.vertSpeed < 0 && Physics.Raycast(replayClone.Clone.transform.position, Vector3.down, out RaycastHit hit))
        {
            float check = (replayClone.Clone.GetComponent<CharacterController>().height + replayClone.Clone.GetComponent<CharacterController>().radius) / 1.9f;
            isCloneHittingGround = hit.distance <= check;
        }
        else
        {
            isCloneHittingGround = false;
        }

        if (isCloneHittingGround)
        {
            if (doJump)
            {
                if (!replayClone.isJumping)
                {
                    replayClone.vertSpeed = relativeMovement.jumpSpeed;
                    replayClone.isJumping = true;
                }
            }
            else
            {
                replayClone.vertSpeed = relativeMovement.minFall;
                replayClone.isJumping = false;
                replayClone.animator.SetBool("Jumping", false);
            }
        }
        else
        {
            replayClone.vertSpeed += relativeMovement.gravity * 5 * Time.fixedDeltaTime;
            replayClone.animator.SetBool("Jumping", true);
            if (replayClone.vertSpeed < relativeMovement.terminalVelocity)
            {
                replayClone.vertSpeed = relativeMovement.terminalVelocity;
            }

            if (replayClone.Clone.GetComponent<CharacterController>().isGrounded)
            {
                if (Vector3.Dot(moveDirection, replayClone.Clone.GetComponent<CloneMovement>()._contact.normal) < 0) // Dot if they point same is 1 (same direction) to -1 (opposite)
                {
                    moveDirection = replayClone.Clone.GetComponent<CloneMovement>()._contact.normal * horSpeed;
                    replayClone.isJumping = false;
                    replayClone.animator.SetBool("Jumping", false);
                }
                else
                {
                    moveDirection += replayClone.Clone.GetComponent<CloneMovement>()._contact.normal * horSpeed * 10;
                }
            }
        }
        moveDirection.y = replayClone.vertSpeed;

        replayClone.Clone.GetComponent<CharacterController>().Move(moveDirection * Time.fixedDeltaTime);
    }

    void CloneAttack(ReplayClone replayClone, bool isLateClone)
    {
        bool doShoot = isLateClone ? replayClone.PlayerRecordings[0].shootPressed : shootPressed;
        bool doMelee = isLateClone ? replayClone.PlayerRecordings[0].meleePressed : meleePressed;
        if (!GameEvent.isPaused)
        {
            //Shooting attack
            if (doShoot)
            {
                GameObject bullet = Instantiate(shooterSystem.GetBulletPrefab()) as GameObject;
                bullet.transform.position = (replayClone.bulletCreationPoint != null) ? replayClone.bulletCreationPoint.transform.position : transform.TransformPoint(Vector3.forward * 2.5f);
                bullet.transform.rotation = replayClone.bulletCreationPoint.transform.rotation;
            }

            //Melee attack
            if (doMelee)
            {
                Vector3 meleePoint = replayClone.Clone.transform.position + replayClone.Clone.transform.forward * 0.8f;
                Collider[] hitColliders = Physics.OverlapSphere(meleePoint, shooterSystem.meleeRadius);
                foreach (var hitCollider in hitColliders)
                {

                    GameObject hitObject = hitCollider.transform.gameObject;
                    IReactiveObject target = hitObject.GetComponent<IReactiveObject>();
                    if (target != null)
                    {
                        target.ReactToHits(1);
                        target.AddHitForce(2, transform.position, shooterSystem.meleeRadius);
                    }
                }
            }
        }
    }

    public void RemoveAndDestroyClone(GameObject clone)
    {
        for (int i = 0; i < _lateClones.Count; i++)
        {
            if (_lateClones[i].Clone == clone)
            {
                _lateClones.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < _mirrorClones.Count; i++)
        {
            if (_mirrorClones[i].Clone == clone)
            {
                _mirrorClones.RemoveAt(i);
                break;
            }
        }

        Destroy(clone);
    }

    public void ClearClones()
    {
        _lateClones.Clear();
        _mirrorClones.Clear();
    }
}
