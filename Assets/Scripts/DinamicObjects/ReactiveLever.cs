using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveLever : MonoBehaviour, IReactiveObject
{
    [SerializeField] public GameObject joint;
    [SerializeField] public ITargetObject target;
    private bool isRightDirection = true;

    //Start coroutine to open box
    public void ReactToHits(int numHits)
    {
        joint.transform.rotation = isRightDirection ? Quaternion.Euler(20f, 0f, 0f) : Quaternion.Euler(-20f, 0f, 0f);
        isRightDirection = !isRightDirection;
        //target.Operate();
    }

    public void AddHitForce(float hitForce, Vector3 hitPosition, float hitRadius)
    {
    }
}
