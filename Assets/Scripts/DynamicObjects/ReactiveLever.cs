using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ReactiveLever : MonoBehaviour, IReactiveObject
{
    [SerializeField] public GameObject joint;
    public TargetObject[] targets;
    private bool isRightDirection = true;

    //Start coroutine to open box
    public void ReactToHits(int numHits)
    {
        joint.transform.rotation = isRightDirection ? Quaternion.Euler(20f, 0f, 0f) : Quaternion.Euler(-20f, 0f, 0f);
        isRightDirection = !isRightDirection;
        if (isRightDirection)
        {
            for(int i = 0; i < targets.Length; i++)
                targets[i].Deactivate();
        }
        else
        {
            for (int i = 0; i < targets.Length; i++)
                targets[i].Activate();
        }
    }

    public void AddHitForce(float hitForce, Vector3 hitPosition, float hitRadius)
    {
    }
}
