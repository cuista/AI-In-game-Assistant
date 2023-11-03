using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    public int damage = 1;

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Start()
    {
        StartCoroutine(DestroyByInactivity());
    }

    void FixedUpdate()
    {
        if (!GameEvent.isPaused)
        {
            transform.Translate(0, 0, speed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Item")) //ignore these tags
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), other);
        }
        else
        {
            //hitted player or enemy
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Clone"))
            {
                other.GetComponent<ICharacter>().Hurt(damage);
            }
            if(other.gameObject.CompareTag("ReactiveObject"))
            {
                other.GetComponent<IReactiveObject>().ReactToHits(1);
            }
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DestroyByInactivity()
    {
        yield return new WaitForSeconds(40);
        Destroy(this.gameObject);
    }
}
