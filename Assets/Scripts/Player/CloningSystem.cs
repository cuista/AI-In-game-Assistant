using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

class PlayerRecord
{
    public readonly Transform playerCamera;
    public readonly bool isFirstPersonView;
    public readonly float horInput, verInput;
    public readonly float moveSpeed;
    public readonly bool jumpPressed;

    public PlayerRecord(Transform playerCamera, bool isFirstPersonView, float horInput, float verInput, float moveSpeed, bool jumpPressed)
    {
        this.playerCamera = playerCamera;
        this.isFirstPersonView = isFirstPersonView;
        this.horInput = horInput;
        this.verInput = verInput;
        this.moveSpeed = moveSpeed;
        this.jumpPressed = jumpPressed;
    }
}

class ReplayClone
{
    public GameObject Clone;
    public List<PlayerRecord> PlayerRecordings;
    public bool isJumping;
    public float vertSpeed;

    public ReplayClone(GameObject clone, List<PlayerRecord> playerRecordings, float vertSpeed)
    {
        this.Clone = clone;
        this.PlayerRecordings = playerRecordings;
        isJumping = false;
        this.vertSpeed = vertSpeed;
    }
}

public class CloningSystem : MonoBehaviour
{
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private GameObject spawnClonePrefab;

    private RelativeMovement relativeMovement;

    private List<ReplayClone> _replayClones;
    private GameObject _spawnPoint;
    private bool _spawnPointTimedOut;
    private List<PlayerRecord> _currentPlayerRecording;

    private bool fire1Pressed;
    private bool fire2Pressed;

    public float timeAcceleration = 3f;

    float horInput;
    float verInput;
    float moveSpeed;
    bool jumpPressed;

    // Start is called before the first frame update
    void Start()
    {
        relativeMovement = GetComponent<RelativeMovement>();

        _replayClones = new List<ReplayClone>();
        _spawnPoint = null;
        _spawnPointTimedOut = false;
        _currentPlayerRecording = null;

        fire1Pressed = false;
        fire2Pressed = false;

        jumpPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Managers.Inventory.GetItemCount("CloneRecharge") > 0)
        {
            if(_spawnPointTimedOut) //SpawnPoint unused for 10 seconds
            {
                Destroy(_spawnPoint);
                _currentPlayerRecording = null;
                _spawnPointTimedOut = false;
            }

            fire1Pressed = Input.GetButtonUp("Fire1") || fire1Pressed;
            fire2Pressed = (Input.GetButtonUp("Fire2") && _spawnPoint != null) || fire2Pressed;
        }

        //Time acceleration ability
        if (Input.GetButtonDown("TimeAcceleration"))
        {
            Time.timeScale = timeAcceleration;
        }
        if (Input.GetButtonUp("TimeAcceleration"))
        {
            Time.timeScale = 1.0f;
        }

        //For input recording
        horInput = Input.GetAxis("Horizontal");
        verInput = Input.GetAxis("Vertical");
        moveSpeed = Input.GetButton("Run") ? relativeMovement.runSpeed: relativeMovement.walkSpeed;
        jumpPressed = Input.GetButtonDown("Jump") || jumpPressed;
    }

    private void FixedUpdate()
    {
        if(fire1Pressed) //Place clone spawn point
        {
            if (_spawnPoint != null)
            {
                Destroy(_spawnPoint);
                _spawnPointTimedOut = false;
                _currentPlayerRecording = new List<PlayerRecord>();
                transform.GetComponent<PlayerCharacter>().ResetUICountdown(); //UI temporary
            }
            else
            {
                _currentPlayerRecording = new List<PlayerRecord>();
                StartCoroutine(transform.GetComponent<PlayerCharacter>().UICountdown(20)); //UI temporary
            }
            _spawnPoint = Instantiate(spawnClonePrefab) as GameObject;
            _spawnPoint.transform.SetLocalPositionAndRotation(transform.position, transform.rotation);
            StartCoroutine(SpawnPointUnused(_spawnPoint));

            fire1Pressed = false;
        }

        if (fire2Pressed) //Spawn the clone and starting replaying player movements
        {
            Managers.Inventory.ConsumeItem("CloneRecharge");

            Transform spawn = _spawnPoint.transform;
            Destroy(_spawnPoint);
            GameObject clone = Instantiate(clonePrefab, spawn.position, spawn.rotation);
            List<PlayerRecord> records = _currentPlayerRecording;
            _replayClones.Add(new ReplayClone(clone, records, relativeMovement.minFall));
            _currentPlayerRecording = null;

            transform.GetComponent<PlayerCharacter>().DisableUICountdown(); //UI temporary

            fire2Pressed = false;
        }

        Transform playerCamera = relativeMovement.PlayerCamera;
        bool isFirstPersonView = relativeMovement.IsFirstPersonView;

        //If there's a clone ready to be placed, a not null clone ready that is recording player movements
        _currentPlayerRecording?.Add(new PlayerRecord(playerCamera, isFirstPersonView, horInput, verInput, moveSpeed, jumpPressed));

        if (_replayClones.Count > 0) //If there's at least a clone in the scene
        {
            foreach (var replayClone in _replayClones)
            {
                //ADD player record
                replayClone.PlayerRecordings.Add(new PlayerRecord(playerCamera, isFirstPersonView, horInput, verInput, moveSpeed, jumpPressed));

                //Clone HORIZONTAL-MOVEMENT
                Vector3 moveDirection = new Vector3(replayClone.PlayerRecordings[0].horInput * replayClone.PlayerRecordings[0].moveSpeed, 0, replayClone.PlayerRecordings[0].verInput * replayClone.PlayerRecordings[0].moveSpeed); //(x,0,z)
                moveDirection = Vector3.ClampMagnitude(moveDirection, replayClone.PlayerRecordings[0].moveSpeed); //avoid diagonal speed-up

                //Handle rotation
                if (moveDirection != Vector3.zero)
                {
                    Transform camera = replayClone.PlayerRecordings[0].playerCamera;
                    camera.eulerAngles = new Vector3(0, camera.eulerAngles.y, 0);
                    moveDirection = camera.TransformDirection(moveDirection);

                    if (!replayClone.PlayerRecordings[0].isFirstPersonView)
                    {
                        Quaternion direction = Quaternion.LookRotation(moveDirection);
                        replayClone.Clone.transform.rotation = Quaternion.Lerp(replayClone.Clone.transform.rotation, direction, 15f * Time.deltaTime); //change rotation smoothly
                    }
                }

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
                    if (replayClone.PlayerRecordings[0].jumpPressed)
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
                    }
                }
                else
                {
                    replayClone.vertSpeed += relativeMovement.gravity * 5 * Time.fixedDeltaTime;
                    if (replayClone.vertSpeed < relativeMovement.terminalVelocity)
                    {
                        replayClone.vertSpeed = relativeMovement.terminalVelocity;
                    }

                    if (replayClone.Clone.GetComponent<CharacterController>().isGrounded)
                    {
                        if (Vector3.Dot(moveDirection, replayClone.Clone.GetComponent<CloneMovement>()._contact.normal) < 0) // Dot if they point same is 1 (same direction) to -1 (opposite)
                        {
                            moveDirection = replayClone.Clone.GetComponent<CloneMovement>()._contact.normal * replayClone.PlayerRecordings[0].moveSpeed;
                            replayClone.isJumping = false;
                        }
                        else
                        {
                            moveDirection += replayClone.Clone.GetComponent<CloneMovement>()._contact.normal * replayClone.PlayerRecordings[0].moveSpeed * 10;
                        }
                    }
                }
                moveDirection.y = replayClone.vertSpeed;

                replayClone.Clone.GetComponent<CharacterController>().Move(moveDirection * Time.fixedDeltaTime);

                //REMOVE player record
                replayClone.PlayerRecordings.RemoveAt(0);
            }
        }
        //Reset jump input recording
        jumpPressed = false;
    }

    private IEnumerator SpawnPointUnused(GameObject spawn) //if a spawn point is unused it will be remove after x seconds
    {
        yield return new WaitForSeconds(20);

        if (spawn != null)
        {
            _spawnPointTimedOut = true;
        }
    }
}
