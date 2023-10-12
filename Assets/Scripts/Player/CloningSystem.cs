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
    //public readonly jumpInput;

    public PlayerRecord(Transform playerCamera, bool isFirstPersonView, float horInput, float verInput, float moveSpeed/*, bool jumpInput*/)
    {
        this.playerCamera = playerCamera;
        this.isFirstPersonView = isFirstPersonView;
        this.horInput = horInput;
        this.verInput = verInput;
        this.moveSpeed = moveSpeed;
        /*
        * this.jumpInput = jumpInput;
        */
    }
}

class ReplayClone
{
    public GameObject Clone;
    public List<PlayerRecord> PlayerRecordings;

    public ReplayClone(GameObject clone, List<PlayerRecord> playerRecordings)
    {
        this.Clone = clone;
        this.PlayerRecordings = playerRecordings;
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
    private int _clonesAvailable;

    private bool fire1Pressed;
    private bool fire2Pressed;

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
        _currentPlayerRecording = new List<PlayerRecord>();
        _clonesAvailable = 8; //Number of max. clones that can be placed

        fire1Pressed = false;
        fire2Pressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_clonesAvailable > 0)
        {
            if(_spawnPointTimedOut) //SpawnPoint unused for 10 seconds
            {
                Destroy(_spawnPoint);
                _spawnPointTimedOut = false;
            }

            fire1Pressed = Input.GetButtonUp("Fire1") || fire1Pressed;
            fire2Pressed = (Input.GetButtonUp("Fire2") && _spawnPoint != null) || fire2Pressed;
        }

        //For input recording
        horInput = relativeMovement.HorInput;
        verInput = relativeMovement.VerInput;
        moveSpeed = relativeMovement.MoveSpeed;
        jumpPressed = Input.GetButtonDown("Jump") || jumpPressed;
    }

    private void FixedUpdate()
    {
        if(fire1Pressed)
        {
            if (_spawnPoint != null)
            {
                Destroy(_spawnPoint);
                _spawnPointTimedOut = false;
                _currentPlayerRecording = null;
                transform.GetComponent<PlayerCharacter>().ResetUICountdown(); //UI temporary
            }
            else
            {
                StartCoroutine(transform.GetComponent<PlayerCharacter>().UICountdown(20)); //UI temporary
            }
            _spawnPoint = Instantiate(spawnClonePrefab) as GameObject;
            _spawnPoint.transform.SetLocalPositionAndRotation(transform.position, transform.rotation);
            StartCoroutine(SpawnPointUnused(_spawnPoint));

            fire1Pressed = false;
        }

        if (fire2Pressed) //Spawn the clone and starting replaying player movements
        {
            Transform spawn = _spawnPoint.transform;
            Destroy(_spawnPoint);
            GameObject clone = Instantiate(clonePrefab, spawn.position, spawn.rotation);
            List<PlayerRecord> records = _currentPlayerRecording;
            _replayClones.Add(new ReplayClone(clone, records));

            _currentPlayerRecording = null;
            _clonesAvailable--;

            transform.GetComponent<PlayerCharacter>().DisableUICountdown(); //UI temporary

            fire2Pressed = false;
        }

        //------------------------------------------------------------------------------------------------------
        Transform playerCamera = relativeMovement.PlayerCamera;
        bool isFirstPersonView = relativeMovement.IsFirstPersonView;

        //If there's a clone ready to be placed, a not null clone ready that is recording player movements
        _currentPlayerRecording?.Add(new PlayerRecord(playerCamera, isFirstPersonView, horInput, verInput, moveSpeed));

        if (_replayClones.Count > 0) //If there's at least a clone in the scene
        {
            foreach (var replayClone in _replayClones)
            {
                //ADD player record
                replayClone.PlayerRecordings.Add(new PlayerRecord(playerCamera, isFirstPersonView, horInput, verInput, moveSpeed));

                //Clone MOVEMENT
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

                /*
                //If player hit the floor or not, handle vertical speed
                float vertSpeed = relativeMovement.minFall;

                bool isCloneHittingGround;
                if (vertSpeed < 0 && Physics.Raycast(replayClone.Clone.transform.position, Vector3.down, out RaycastHit hit))
                {
                    float check = (replayClone.Clone.GetComponent<CharacterController>().height + replayClone.Clone.GetComponent<CharacterController>().radius) / 1.9f;
                    isCloneHittingGround = hit.distance <= check;
                }
                else
                {
                    isCloneHittingGround = false;
                }
                Debug.Log("CLONE TOCCA TERRA?: " + isCloneHittingGround);
                */
                /*
                if (isCloneHittingGround)
                {
                    if (jumpPressed)
                    {
                        jumpPressed = false;
                        vertSpeed = relativeMovement.jumpSpeed;
                    }
                    else
                    {
                        vertSpeed = relativeMovement.minFall;
                    }
                }
                else
                {
                    vertSpeed += relativeMovement.gravity * 5 * Time.fixedDeltaTime;
                    if (vertSpeed < relativeMovement.terminalVelocity)
                    {
                        vertSpeed = relativeMovement.terminalVelocity;
                    }

                    if (replayClone.Clone.GetComponent<CharacterController>().isGrounded)
                    {
                        if (Vector3.Dot(moveDirection, replayClone.Clone.GetComponent<CloneMovement>()._contact.normal) < 0) // Dot if they point same is 1 (same direction) to -1 (opposite)
                        {
                            moveDirection = replayClone.Clone.GetComponent<CloneMovement>()._contact.normal * moveSpeed;
                        }
                        else
                        {
                            moveDirection += replayClone.Clone.GetComponent<CloneMovement>()._contact.normal * moveSpeed * 10;
                        }
                    }
                }
                moveDirection.y = vertSpeed;
                */

                replayClone.Clone.GetComponent<CharacterController>().Move(moveDirection * Time.fixedDeltaTime);

                //REMOVE player record
                replayClone.PlayerRecordings.RemoveAt(0);
            }
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
}
