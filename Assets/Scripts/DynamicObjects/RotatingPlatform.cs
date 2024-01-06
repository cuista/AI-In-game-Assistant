using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : TargetObject
{
    private bool isOperating = false;
    private Quaternion targetRotation;
    [SerializeField] float speed = 20f;

    // Update is called once per frame
    void FixedUpdate() //Script Execution Order => >300
    {
        if (isOperating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.fixedDeltaTime);

            //If it's close to the final position, invert direction
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.01f)
            {
                transform.rotation = targetRotation;
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
        Operate();
    }

    public override void Deactivate()
    {
        Operate();
    }

    public override void Operate()
    {
        if(!isOperating)
        {
            targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 90f, transform.rotation.eulerAngles.z);
            isOperating = true;
        }
    }
}
