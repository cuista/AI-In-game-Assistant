using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformMovement : TargetObject
{
    [SerializeField] Vector3 targetPosition;
    private Vector3 originPosition;
    private Vector3 _finalPos;
    public bool isOperating = false;
    [SerializeField] float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.position;
        _finalPos = targetPosition;
    }

    // Update is called once per frame
    void FixedUpdate() //Script Execution Order => 300
    {
        if (isOperating)
        {
            transform.position = Vector3.MoveTowards(transform.position, _finalPos, speed * Time.fixedDeltaTime);

            //If it's close to the final position, invert direction
            if (Vector3.Distance(transform.position, _finalPos) < 0.01f)
            {
                isOperating = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }


    public override void Activate()
    {
        isOperating = true;

        _finalPos = targetPosition;
    }

    public override void Deactivate()
    {
        isOperating = true;

        _finalPos = originPosition;
    }

    public override void Operate()
    {
        isOperating = !isOperating;

        _finalPos = _finalPos==originPosition?targetPosition:originPosition;
    }
}
