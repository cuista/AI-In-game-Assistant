using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveBox : MonoBehaviour, IReactiveObject
{
    [SerializeField] public GameObject item;
    //[SerializeField] public GameObject explosionEffect;

    //Start coroutine to open box
    public void ReactToHits(int numHits)
    {
        StartCoroutine(Open());
    }

    //Open the box with explosion effect
    private IEnumerator Open()
    {
        //ExplosionController.MakeItBoom(explosionEffect, transform);

        yield return new WaitForSeconds(0.5f);

        Instantiate(item, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
        Destroy(this.gameObject);
    }

    public void AddHitForce(float hitForce, Vector3 hitPosition, float hitRadius)
    {
    }
}
