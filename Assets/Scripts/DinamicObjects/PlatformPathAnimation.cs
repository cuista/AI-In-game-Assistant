using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPathAnimation : MonoBehaviour
{
    [SerializeField] Vector3[] targetPosition;
    private int indexTarget;

    private bool _isActivated;
    private Vector3 _initialPos;

    [SerializeField] float speed = 8.0f;

    // Start is called before the first frame update
    private void Start()
    {
        _initialPos = transform.position;
        indexTarget = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if actived and indexTarget in correct bounds
        if (_isActivated && indexTarget >= 0 && indexTarget < targetPosition.Length)
        {
            float step = speed * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition[indexTarget], step);

            if (Vector3.Distance(transform.position, targetPosition[indexTarget]) < 0.01f)
            {
                if (indexTarget < targetPosition.Length - 1)
                {
                    indexTarget++;
                }
            }
        }
        else
        {
            // When the device is not activated or has completed the route, I return it to the initialPos
            float step = speed * Time.fixedDeltaTime;

            if (indexTarget > 0)
            {
                // If the index is greater than zero, I move the device to the previous target in reverse order
                transform.position = Vector3.MoveTowards(transform.position, targetPosition[indexTarget - 1], step);
                if (Vector3.Distance(transform.position, targetPosition[indexTarget - 1]) < 0.01f)
                {
                    indexTarget--;
                }
            }
            else
            {
                // Se l'indice è zero o meno, muovo il dispositivo verso _initialPos
                transform.position = Vector3.MoveTowards(transform.position, _initialPos, step);
                if (Vector3.Distance(transform.position, _initialPos) < 0.01f)
                {
                    indexTarget = 0;
                }
            }
        }
    }

    public void Activate()
    {
        _isActivated = true;
    }

    public void Deactivate()
    {
        _isActivated = false;
    }
}
