using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveBox : MonoBehaviour, IReactiveObject
{
    [SerializeField] public GameObject item;
    [SerializeField] public GameObject explosionEffect;
    [SerializeField] public GameObject hitEffect;

    private bool _isOpen;
    public int _strenght = 3;

    private Animator animator;

    private void Start()
    {
        _isOpen = false;
        animator = GetComponent<Animator>();
    }

    //Start coroutine to open box
    public void ReactToHits(int numHits)
    {
        if (!_isOpen)
        {
            animator.SetTrigger("Open");
            _isOpen = true;
        }
        else
        {
            _strenght = _strenght>0 ? _strenght - 1 : RemoveBox();
        }
        ExplosionController.MakeItBoom(hitEffect, transform);
    }

    public void DropItem()
    {
        Debug.Log(transform.rotation.y);
        switch(transform.rotation.y)
        {
            case 1f:
                Instantiate(item, new Vector3(transform.position.x - 1f, transform.position.y + 0.4f, transform.position.z), transform.rotation);
                break;
            case <1f and >0f:
                Instantiate(item, new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z -1f), transform.rotation);
                break;
            case <0f and >-1f:
                Instantiate(item, new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z +1f), transform.rotation);
                break;
            case 0f: default:
                Instantiate(item, new Vector3(transform.position.x + 1f, transform.position.y + 0.4f, transform.position.z), transform.rotation); 
                break;
        }
    }

    private int RemoveBox()
    {
        StartCoroutine(RemoveWithExplosion());
        return 0;
    }

    //Open the box with explosion effect
    private IEnumerator RemoveWithExplosion()
    {
        ExplosionController.MakeItBoom(explosionEffect, transform);
        yield return new WaitForSeconds(0.5f);

        Destroy(this.gameObject);
    }

    public void AddHitForce(float hitForce, Vector3 hitPosition, float hitRadius)
    {
    }
}
