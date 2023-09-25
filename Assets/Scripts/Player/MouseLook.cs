using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Only for first person view
public class MouseLook : MonoBehaviour
{
    public enum RotationAxes
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;

    private float _rotationX = 0;

    /*
    [SerializeField] private GameObject _head;
    private float _headRotationX = 0;
    */

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
        if (axes == RotationAxes.MouseXAndY)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);

            /*
            _headRotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            _headRotationX = Mathf.Clamp(_headRotationX, minimumVert, maximumVert);

            _head.transform.localEulerAngles = new Vector3(_headRotationX, 0, 0);
            */
        }
        else if (axes == RotationAxes.MouseY)
        {
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            float rotationY = transform.localEulerAngles.y;

            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
        else
        {
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            float delta = Input.GetAxis("Mouse Y") * sensitivityHor;
            float rotationY = transform.localEulerAngles.y + delta;

            transform.localEulerAngles = new Vector3 (_rotationX, rotationY, 0);
        }

    }

    private void OnDisable()
    {
        /*
        _head.transform.localEulerAngles = new Vector3(0, 0, 0);
        _headRotationX = 0;
        */
    }
}
