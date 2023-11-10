using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTarget : MonoBehaviour
{
    [SerializeField] private float gap;
    [SerializeField] private GameObject doorRight;
    [SerializeField] private GameObject doorLeft;

    private Vector3 _closePosRight;
    private Vector3 _closePosLeft;
    private Vector3 _openPosRight;
    private Vector3 _openPosLeft;

    private bool _open;
    private bool _doorsAreMoving;

    [SerializeField] float speed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        _closePosRight = doorRight.transform.position;
        _closePosLeft = doorLeft.transform.position;
        _openPosRight = new Vector3(_closePosRight.x + gap, _closePosRight.y, _closePosRight.z);
        _openPosLeft = new Vector3(_closePosLeft.x - gap, _closePosLeft.y, _closePosLeft.z);
    }

    public void Activate()
    {
        if (!_open)
        {
            _doorsAreMoving = true;
        }
        else if (_doorsAreMoving)
        {
            _open = false;
        }
    }

    public void Deactivate()
    {
        if (_open)
        {
            _doorsAreMoving = true;
        }
        else if (_doorsAreMoving)
        {
            _open = true;
        }
    }

    public void Operate()
    {
        if (!_open)
        {
            if (doorRight.transform.position == _openPosRight && doorLeft.transform.position == _openPosLeft)
            {
                _doorsAreMoving = false;
                _open = true;
            }
            else
            {
                doorRight.transform.position = Vector3.Lerp(doorRight.transform.position, _openPosRight, speed * Time.deltaTime);
                doorLeft.transform.position = Vector3.Lerp(doorLeft.transform.position, _openPosLeft, speed * Time.deltaTime);

            }
        }
        else
        {
            if (doorRight.transform.position == _closePosRight && doorLeft.transform.position == _closePosLeft)
            {
                _doorsAreMoving = false;
                _open = false;
            }
            else
            {
                doorRight.transform.position = Vector3.Lerp(doorRight.transform.position, _closePosRight, speed * Time.deltaTime);
                doorLeft.transform.position = Vector3.Lerp(doorLeft.transform.position, _closePosLeft, speed * Time.deltaTime);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_doorsAreMoving)
        {
            Operate();
        }
    }
}
