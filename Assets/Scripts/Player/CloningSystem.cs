using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject originPoint;

    public float recordingTimer = 0;
    public int replayIndex = 0;

    public Animator animator;

    public ReplayClone(GameObject clone, List<PlayerRecord> playerRecordings, float vertSpeed, GameObject originPoint) //for late clones
    {
        this.Clone = clone;
        this.PlayerRecordings = playerRecordings;
        isJumping = false;
        this.vertSpeed = vertSpeed;
        bulletCreationPoint = clone.GetComponent<CloneCharacter>().GetBulletCreationPoint();
        head = clone.GetComponentInChildren<CloneHead>().gameObject;
        this.originPoint = originPoint;

        animator = clone.GetComponent<Animator>();
    }

    public ReplayClone(GameObject clone, float vertSpeed) //for mirror clones
    {
        this.Clone = clone;
        isJumping = false;
        this.vertSpeed = vertSpeed;
        bulletCreationPoint = clone.GetComponent<CloneCharacter>().GetBulletCreationPoint();
        head = clone.GetComponentInChildren<CloneHead>().gameObject;

        animator = clone.GetComponent<Animator>();
    }

    public void RegenerateClone(GameObject clone)
    {
        this.Clone = clone;
        replayIndex = 0;
        animator = clone.GetComponent<Animator>();
    }
}

public class CloningSystem : MonoBehaviour
{
    [SerializeField] private GameObject lateClonePrefab;
    [SerializeField] private GameObject mirrorClonePrefab;
    [SerializeField] private GameObject spawnPointPrefab;
    [SerializeField] private GameObject originPointPrefab;

    private RelativeMovement relativeMovement;
    private FirstPersonLook firstPersonLook;
    private CombatSystem shooterSystem;

    private List<ReplayClone> _lateClones;
    private List<ReplayClone> _mirrorClones;
    private GameObject _spawnPoint;
    private bool _spawnPointTimedOut;
    private List<PlayerRecord> _currentPlayerRecording;

    private bool fire2Pressed;
    private bool fire1Pressed;

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

    private Animator _animator;

    private AudioSource _audioSource;

    [SerializeField] private AudioClip plantSpawnSound;
    [SerializeField] private AudioClip cloneSpawnSound;

    // Start is called before the first frame update
    void Start()
    {
        relativeMovement = GetComponent<RelativeMovement>();
        firstPersonLook = GetComponent<FirstPersonLook>();
        shooterSystem = GetComponent<CombatSystem>();

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
                if(_spawnPointTimedOut) //SpawnPoint unused after the timelimit seconds
                {
                    Messenger.Broadcast(GameEvent.SPAWN_POINT_EXPIRED);

                    Destroy(_spawnPoint);
                    _currentPlayerRecording = null;
                    _spawnPointTimedOut = false;
                }

                fire1Pressed = (Input.GetButtonUp("Fire1") && _spawnPoint != null) || fire1Pressed;
                fire2Pressed = Input.GetButtonUp("Fire2") || fire2Pressed;
            }

            //Switch late and mirror clone
            if (Input.GetButtonUp("SwitchCloneMode"))
            {
                _isLateCloneMode = !_isLateCloneMode;
                Messenger.Broadcast(GameEvent.SWITCHED_CLONE_MODE);
            }

            //For input recording
            mouseX = Input.GetAxis("Mouse X");
            horInput = Input.GetAxis("Horizontal");
            verInput = Input.GetAxis("Vertical");
            moveSpeed = Input.GetButton("Walk") ? relativeMovement.walkSpeed: relativeMovement.runSpeed;
            jumpPressed = Input.GetButtonDown("Jump") || jumpPressed;
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
                _spawnPoint = Instantiate(spawnPointPrefab) as GameObject;
                _spawnPoint.transform.position = new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z);
                _spawnPoint.transform.rotation = transform.rotation;
                transform.GetComponent<PlayerCharacter>().SetSpawnCloneAnimator(_spawnPoint.GetComponent<Animator>()); //Flash spawnClone Animation
                StartCoroutine(SpawnPointUnused(_spawnPoint));

                fire2Pressed = false;

                Messenger.Broadcast(GameEvent.SPAWN_POINT_PLACED);

                _animator.SetTrigger("PlantSpawn");
                _audioSource.PlayOneShot(plantSpawnSound);
            }

            if (fire1Pressed) //Spawn the clone and starting replaying player movements
            {
                Managers.Inventory.ConsumeItem("CloneRecharge");

                Messenger.Broadcast(GameEvent.SPAWN_POINT_EXPIRED);

                Transform spawn = _spawnPoint.transform;
                Destroy(_spawnPoint);
                GameObject originPoint = Instantiate(originPointPrefab, spawn.position, spawn.rotation);

                if (_isLateCloneMode)
                {
                    GameObject clone = Instantiate(lateClonePrefab, spawn.position, spawn.rotation);
                    List<PlayerRecord> records = _currentPlayerRecording;
                    _lateClones.Add(new ReplayClone(clone, records, relativeMovement.minFall, originPoint));
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
                        if (replayClone.recordingTimer < 25f)
                        {
                            //INCREASE recordingTimer
                            replayClone.recordingTimer += Time.deltaTime;
                            
                            //ADD player record
                            replayClone.PlayerRecordings.Add(new PlayerRecord(_playerCamera, _isFirstPersonView, _headRotationX, mouseX, horInput, verInput, moveSpeed, jumpPressed, shootPressed, meleePressed));
                        }

                        //Clone replays recorded player movements
                        CloneMovement(replayClone, true);

                        //Clone replays recorded player attacks
                        CloneAttack(replayClone, true);

                        //INCREASE replayIndex value
                        if (replayClone.replayIndex < replayClone.PlayerRecordings.Count - 1)
                        {
                            replayClone.replayIndex++;
                        }
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
        int i = replayClone.replayIndex;

        Transform playerCamera = isLateClone ? replayClone.PlayerRecordings[i].playerCamera : _playerCamera;
        bool isFirstPersonView = isLateClone ? replayClone.PlayerRecordings[i].isFirstPersonView : _isFirstPersonView;
        float headRotX = isLateClone ? replayClone.PlayerRecordings[i].headRotationX : _headRotationX;
        float rotX = isLateClone ? replayClone.PlayerRecordings[i].mouseX : mouseX;
        float hor = isLateClone ? replayClone.PlayerRecordings[i].horInput : horInput;
        float ver = isLateClone ? replayClone.PlayerRecordings[i].verInput : verInput;
        float horSpeed = isLateClone ? replayClone.PlayerRecordings[i].moveSpeed : moveSpeed;
        bool doJump = isLateClone ? replayClone.PlayerRecordings[i].jumpPressed : jumpPressed;

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
                CloneMovement cloneMovement = replayClone.Clone.GetComponent<CloneMovement>();
                if(cloneMovement!=null || cloneMovement._contact.normal != null) // If grounded record but flying at spawn
                {
                    if (Vector3.Dot(moveDirection, cloneMovement._contact.normal) < 0) // Dot if they point same is 1 (same direction) to -1 (opposite)
                    {
                        moveDirection = replayClone.Clone.GetComponent<CloneMovement>()._contact.normal * horSpeed;
                        replayClone.isJumping = false;
                        replayClone.animator.SetBool("Jumping", false);
                    }
                    else
                    {
                        moveDirection += cloneMovement._contact.normal * horSpeed * 10;
                    }
                }
            }
        }
        moveDirection.y = replayClone.vertSpeed;

        replayClone.Clone.GetComponent<CharacterController>().Move(moveDirection * Time.fixedDeltaTime);
    }

    void CloneAttack(ReplayClone replayClone, bool isLateClone)
    {
        int i = replayClone.replayIndex;

        bool doShoot = isLateClone ? replayClone.PlayerRecordings[i].shootPressed : shootPressed;
        bool doMelee = isLateClone ? replayClone.PlayerRecordings[i].meleePressed : meleePressed;
        if (!GameEvent.isPaused)
        {
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

                switch ((int)Random.Range(0, 3))
                {
                    case 0: replayClone.animator.SetTrigger("Melee1"); break;
                    case 1: replayClone.animator.SetTrigger("Melee2"); break;
                    case 2: default: replayClone.animator.SetTrigger("Melee3"); break;
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
                Transform origin = _lateClones[i].originPoint.transform;
                _lateClones[i].RegenerateClone(Instantiate(lateClonePrefab, origin.position, origin.rotation));
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
