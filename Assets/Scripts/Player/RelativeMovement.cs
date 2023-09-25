using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeMovement : MonoBehaviour
{
    [SerializeField] private Transform target;

    public float rotSpeed = 15.0f;

    private float _moveSpeed = 10.0f;
    private CharacterController _charController;

    public float jumpSpeed = 15.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -10.0f;
    public float minFall = -1.5f;
    private float _vertSpeed;
    private bool _isJumping;

    public const float baseSpeed = 6.0f;
    private ControllerColliderHit _contact; //to be precise on edge of objects

    [SerializeField] CinemachineVirtualCameraBase thirdPersonCamera;
    [SerializeField] CinemachineVirtualCameraBase firstPersonCamera;

    // Start is called before the first frame update
    void Start()
    {
        _vertSpeed = minFall;
        _isJumping = false;
        _charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
            Vector3 movement = Vector3.zero;
            float horInput = Input.GetAxis("Horizontal");
            float vertInput = Input.GetAxis("Vertical");

            if (horInput != 0 || vertInput != 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _moveSpeed = 9f;
                }
                else
                {
                    _moveSpeed = 6f;
                }
                movement.x = horInput * _moveSpeed;
                movement.z = vertInput * _moveSpeed;
                movement = Vector3.ClampMagnitude(movement, _moveSpeed); //avoid diagonal speed-up
                Quaternion tmp = target.rotation;
                target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
                movement = target.TransformDirection(movement);
                if (thirdPersonCamera.Priority == 10)
                {
                    target.rotation = tmp;
                    Quaternion direction = Quaternion.LookRotation(movement);
                    transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime); //change rotation smoothly
                }
            }

            bool hitGround = false;
            RaycastHit hit;
            if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
            {
                float check = (_charController.height + _charController.radius) / 1.9f;
                hitGround = hit.distance <= check;
            }

            //Player is hitting the floor
            if (hitGround)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    _vertSpeed = jumpSpeed;
                }
                else
                {
                    _vertSpeed = minFall;
                    _isJumping = false;
                }
            }
            else
            {
                _vertSpeed += gravity * 5 * Time.deltaTime;
                if (_vertSpeed < terminalVelocity)
                {
                    _vertSpeed = terminalVelocity;
                }

                _isJumping = true;

                if (_charController.isGrounded)
                {
                    if (Vector3.Dot(movement, _contact.normal) < 0) // Dot if they point same is 1 (same direction) to -1 (opposite)
                    {
                        movement = _contact.normal * _moveSpeed;
                        _isJumping = false;
                    }
                    else
                    {
                        movement += _contact.normal * _moveSpeed * 10;
                    }
                }
            }
            movement.y = _vertSpeed;

            _charController.Move(movement * Time.deltaTime);

            //Switch camera view
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if(CameraSwitcher.IsActiveCamera(thirdPersonCamera))
                {
                    CameraSwitcher.SwitchCamera(firstPersonCamera);
                }
                else if(CameraSwitcher.IsActiveCamera(firstPersonCamera))
                {
                    CameraSwitcher.SwitchCamera(thirdPersonCamera);
                }
            }
    }
    public bool isJumping()
    {
        return _isJumping;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }

    private void OnSpeedChanged(float value)
    {
        _moveSpeed = baseSpeed * value;
    }

    private void OnEnable()
    {
        CameraSwitcher.Register(thirdPersonCamera);
        CameraSwitcher.Register(firstPersonCamera);
        CameraSwitcher.SwitchCamera(firstPersonCamera);
    }

    private void OnDisable()
    {
        CameraSwitcher.Unregister(thirdPersonCamera);
        CameraSwitcher.Unregister(firstPersonCamera); 
    }
}
