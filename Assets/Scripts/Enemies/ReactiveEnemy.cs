using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveEnemy : MonoBehaviour, IReactiveObject
{

    //[SerializeField] public GameObject explosionEffect; //TODO for visual explosions when hitted

    public void ReactToHits(int numHits)
    {
        IEnemy enemy = GetComponent<IEnemy>();
        if (enemy != null)
        {
            enemy.RemoveLives(numHits);
            //ExplosionController.MakeItBoom(explosionEffect, transform);
            if (enemy.GetLives() < 1)
            {
                enemy.SetMoving(false);
                StartCoroutine(Die());
            }
        }
    }

    private IEnumerator Die()
    {
        this.transform.Rotate(-75, 0, 0);
        //ExplosionController.MakeItBoom(explosionEffect, transform);

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody.isKinematic)
        {
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        //ExplosionController.MakeFloorOnFire(transform);

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        //ExplosionController.MakeItBoom(explosionEffect, transform);
        Destroy(this.gameObject);
    }

    public void AddHitForce(float hitForce, Vector3 hitPosition, float hitRadius)
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.AddExplosionForce(1000f, hitPosition, hitRadius);
        }
    }
}
