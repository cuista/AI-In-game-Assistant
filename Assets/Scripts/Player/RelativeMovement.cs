using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeMovement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;

    private float _horInput;
    private float _vertInput;
    private bool _jumpPressed;
    private float _moveSpeed = 10.0f;
    private CharacterController _charController;

    public float rotSpeed = 15.0f;
    public float walkSpeed = 6f;
    public float runSpeed = 9f;
    public float jumpSpeed = 15f;
    public float gravity = -9.8f;
    public float terminalVelocity = -15f;
    public float minFall = -1.5f;
    private float _vertSpeed;
    private bool _isJumping;

    public const float baseSpeed = 6.0f;
    private Vector3 moveDirection = Vector3.zero;
    private ControllerColliderHit _contact; //To be precise on edge of objects


    [SerializeField] CinemachineVirtualCameraBase firstPersonCamera;
    [SerializeField] CinemachineVirtualCameraBase thirdPersonCamera;
    [SerializeField] CinemachineVirtualCameraBase isometricCamera;

    private bool _isFirstPersonView;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _vertSpeed = minFall;
        _isJumping = false;
        _charController = GetComponent<CharacterController>();
        _jumpPressed = false;
        SwitchCameraView(3);
    }

    // Update is called once per frame
    void Update()
    {
        //Switch camera view
        HandleCameraView();

        //Horizontal and vertical
        _horInput = Input.GetAxis("Horizontal");
        _vertInput = Input.GetAxis("Vertical");

        //If jump pressed, it can be set false in fixedUpdate
        _jumpPressed = Input.GetButtonDown("Jump") || _jumpPressed;

        //Change player speed
        _moveSpeed = Input.GetButton("Run") ? runSpeed : walkSpeed;
    }

    void FixedUpdate()
    {
        moveDirection = new Vector3(_horInput * _moveSpeed, 0, _vertInput * _moveSpeed); //(x,0,z)
        moveDirection = Vector3.ClampMagnitude(moveDirection, _moveSpeed); //avoid diagonal speed-up

        //Handle rotation
        if (moveDirection != Vector3.zero)
        {
            playerCamera.eulerAngles = new Vector3(0, playerCamera.eulerAngles.y, 0);
            moveDirection = playerCamera.TransformDirection(moveDirection);

            if (!IsFirstPersonView)
            {
                Quaternion direction = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime); //change rotation smoothly
            }
        }

        //If player hit the floor or not, handle vertical speed
        if (IsHittingGround())
        {
            if (_jumpPressed)
            {
                _jumpPressed = false;
                if(!IsJumping)
                {
                    _vertSpeed = jumpSpeed;
                    _isJumping = true;
                }
            }
            else
            {
                _vertSpeed = minFall;
                _isJumping = false;
            }
        }
        else
        {
            _jumpPressed = false;
            _vertSpeed += gravity * 5 * Time.fixedDeltaTime;
            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity;
            }

            if (_contact != null && _charController.isGrounded)
            {
                if (Vector3.Dot(moveDirection, _contact.normal) < 0) // Dot if they point same is 1 (same direction) to -1 (opposite)
                {
                    moveDirection = _contact.normal * _moveSpeed;
                    _isJumping = false;
                }
                else
                {
                    moveDirection += _contact.normal * _moveSpeed * 10;
                }
            }
        }
        moveDirection.y = _vertSpeed;

        _charController.Move(moveDirection * Time.fixedDeltaTime);
    }

    public Transform PlayerCamera { get => playerCamera; }
    public float HorInput { get => _horInput; }
    public float VerInput { get => _vertInput; }
    public float MoveSpeed { get => _moveSpeed; }
    public bool JumpPressed { get => _jumpPressed; }
    public bool IsJumping { get => _isJumping; }
    public bool IsFirstPersonView { get => _isFirstPersonView; }

    public bool IsHittingGround()
    {
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            float check = (_charController.height + _charController.radius) / 1.9f;
            return hit.distance <= check;
        }
        return false;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }

    private void HandleCameraView()
    {
        if (Input.GetButtonDown("C"))
        {
            if (CameraSwitcher.IsActiveCamera(firstPersonCamera))
            {
                SwitchCameraView(2);
            }
            else if (CameraSwitcher.IsActiveCamera(thirdPersonCamera))
            {
                SwitchCameraView(3);
            }
            else
            {
                SwitchCameraView(1);
            }
        }
        else if (Input.GetButtonDown("1"))
        {
            SwitchCameraView(1);
        }
        else if (Input.GetButtonDown("2"))
        {
            SwitchCameraView(2);
        }
        else if (Input.GetButtonDown("3"))
        {
            SwitchCameraView(3);
        }
    }

    private void SwitchCameraView(int v)
    {
        switch (v)
        {
            case 1: CameraSwitcher.SwitchCamera(firstPersonCamera); _isFirstPersonView = true; break;
            case 2: CameraSwitcher.SwitchCamera(thirdPersonCamera); _isFirstPersonView = false; break;
            case 3: default: CameraSwitcher.SwitchCamera(isometricCamera); _isFirstPersonView = false; break;
        }
    }

    private void OnEnable()
    {
        CameraSwitcher.Register(firstPersonCamera);
        CameraSwitcher.Register(thirdPersonCamera);
        CameraSwitcher.Register(isometricCamera);
        SwitchCameraView(3);
    }

    private void OnDisable()
    {
        CameraSwitcher.Unregister(firstPersonCamera);
        CameraSwitcher.Unregister(thirdPersonCamera);
        CameraSwitcher.Unregister(isometricCamera);
    }
}
