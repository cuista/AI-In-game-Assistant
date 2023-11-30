using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlatformMovement : TargetObject
{
    [SerializeField] Vector3 targetPosition;
    private Vector3 _startingPos;
    private Vector3 _finalPos;
    public bool isOperating = false;
    [SerializeField] float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        _startingPos = transform.position;
        _finalPos = _finalPos + targetPosition;
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
                //Swap final and starting point
                Vector3 temp = _startingPos;
                _startingPos = _finalPos;
                _finalPos = temp;
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
    }

    public override void Deactivate()
    {
        isOperating = false;
    }

    public override void Operate()
    {
        isOperating = !isOperating;
    }
}
