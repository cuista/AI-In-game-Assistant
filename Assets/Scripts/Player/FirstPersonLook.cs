using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Only for first person view
public class FirstPersonLook : MonoBehaviour
{
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private float _rotX = 0;
    private float _rotY = 0;

    public float headRotationX = 0;

    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _nose;
    [SerializeField] private GameObject _bulletCreationPoint;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
            body.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        _rotX = Input.GetAxis("Mouse X");
        _rotY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, _rotX * sensitivityHor, 0);

        headRotationX -= _rotY * sensitivityVert;
        headRotationX = Mathf.Clamp(headRotationX, minimumVert, maximumVert);

        _head.transform.localEulerAngles = new Vector3(headRotationX, 0, 0);
    }

    private void OnEnable()
    {
        _bulletCreationPoint.transform.SetParent(_nose.transform);
    }

    private void OnDisable()
    {
        _head.transform.localEulerAngles = new Vector3(0, 0, 0);
        headRotationX = 0;

        _bulletCreationPoint.transform.SetParent(_head.transform);
    }
}
