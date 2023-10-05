using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeMovement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;

    public float rotSpeed = 15.0f;

    private float _moveSpeed = 10.0f;
    private CharacterController _charController;

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

    private bool jumpPressed;

    [SerializeField] CinemachineVirtualCameraBase firstPersonCamera;
    [SerializeField] CinemachineVirtualCameraBase thirdPersonCamera;
    [SerializeField] CinemachineVirtualCameraBase isometricCamera;

    // Start is called before the first frame update
    void Start()
    {
        _vertSpeed = minFall;
        _isJumping = false;
        _charController = GetComponent<CharacterController>();
        jumpPressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        jumpPressed = Input.GetButtonDown("Jump") && (!IsJumping()) || jumpPressed;

        //Switch camera view
        HandleCameraView();

        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");

        //Change player speed
        _moveSpeed = Input.GetButton("Run") ? runSpeed : walkSpeed;

        moveDirection = new Vector3(horInput * _moveSpeed, 0, vertInput * _moveSpeed); //(x,0,z)
        moveDirection = Vector3.ClampMagnitude(moveDirection, _moveSpeed); //avoid diagonal speed-up

        //Handle rotation
        if (moveDirection != Vector3.zero)
        {
            Quaternion tmp = playerCamera.rotation;
            playerCamera.eulerAngles = new Vector3(0, playerCamera.eulerAngles.y, 0);
            moveDirection = playerCamera.TransformDirection(moveDirection);

            if (thirdPersonCamera.Priority == 10 || isometricCamera.Priority == 10)
            {
                Quaternion direction = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime); //change rotation smoothly
            }
        }
    }

    void FixedUpdate()
    {
        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (_charController.height + _charController.radius) / 1.9f;
            hitGround = hit.distance <= check;
        }

        //If player hit the floor or not, handle vertical speed
        if (hitGround)
        {
            if (jumpPressed)
            {
                jumpPressed = false;
                _vertSpeed = jumpSpeed;
                _isJumping = true;
            }
            else
            {
                _vertSpeed = minFall;
                _isJumping = false;
            }
        }
        else
        {
            _vertSpeed += gravity * 5 * Time.fixedDeltaTime;
            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity;
            }

            if (_charController.isGrounded)
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

    public bool IsJumping()
    {
        return _isJumping;
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
                CameraSwitcher.SwitchCamera(thirdPersonCamera);
            }
            else if (CameraSwitcher.IsActiveCamera(thirdPersonCamera))
            {
                CameraSwitcher.SwitchCamera(isometricCamera);
            }
            else
            {
                CameraSwitcher.SwitchCamera(firstPersonCamera);
            }
        }
        else if (Input.GetButtonDown("1"))
        {
            CameraSwitcher.SwitchCamera(firstPersonCamera);
        }
        else if (Input.GetButtonDown("2"))
        {
            CameraSwitcher.SwitchCamera(thirdPersonCamera);
        }
        else if (Input.GetButtonDown("3"))
        {
            CameraSwitcher.SwitchCamera(isometricCamera);
        }
    }

    private void OnEnable()
    {
        CameraSwitcher.Register(firstPersonCamera);
        CameraSwitcher.Register(thirdPersonCamera);
        CameraSwitcher.Register(isometricCamera);
        CameraSwitcher.SwitchCamera(isometricCamera);
    }

    private void OnDisable()
    {
        CameraSwitcher.Unregister(firstPersonCamera);
        CameraSwitcher.Unregister(thirdPersonCamera);
        CameraSwitcher.Unregister(isometricCamera);
    }
}
