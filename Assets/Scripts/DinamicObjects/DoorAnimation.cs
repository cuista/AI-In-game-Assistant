using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{
    [SerializeField] private Vector3 dPos;

    private Vector3 _closePos;
    private Vector3 _openPos;

    private bool _open;
    private bool _doorIsMoving;

    [SerializeField] float speed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        _closePos = transform.position;
        _openPos = _closePos + dPos;
    }

    public void Operate()
    {
        if (!_open)
        {
            if (!_doorIsMoving)
            {
                _doorIsMoving = true;
            }
            else
            {
                _open = true;
            }
        }
        else
        {
            if (!_doorIsMoving)
            {
                _doorIsMoving = true;
            }
            else
            {
                _open = false;
            }
        }
    }

    public void Activate()
    {
        if (!_open)
        {
            _doorIsMoving = true;
        }
        else if (_doorIsMoving)
        {
            _open = false;
        }
    }

    public void Deactivate()
    {
        if (_open)
        {
            _doorIsMoving = true;
        }
        else if (_doorIsMoving)
        {
            _open = true;
        }
    }

    void doorAnimation()
    {
        if (!_open)
        {
            if (transform.position != _openPos)
            {
                transform.position = Vector3.Lerp(transform.position, _openPos, speed * Time.deltaTime);
            }
            else
            {
                _doorIsMoving = false;
                _open = true;
            }
        }
        else
        {
            if (transform.position != _closePos)
            {
                transform.position = Vector3.Lerp(transform.position, _closePos, speed * Time.deltaTime);
            }
            else
            {
                _doorIsMoving = false;
                _open = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_doorIsMoving)
        {
            doorAnimation();
        }
    }
}
