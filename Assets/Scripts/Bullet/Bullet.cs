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
            Destroy(this.gameObject);
        }
    }
}
