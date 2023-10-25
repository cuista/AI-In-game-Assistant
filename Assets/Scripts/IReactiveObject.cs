using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReactiveObject
{
    void ReactToHits(int numHits);

    void AddHitForce(float hitForce, Vector3 hitPosition, float hitRadius);
}
