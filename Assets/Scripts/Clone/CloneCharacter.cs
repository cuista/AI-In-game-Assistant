using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneCharacter : MonoBehaviour, ICharacter
{
    private CapsuleCollider _capsuleCollider;
    private bool _isHittingGround;

    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _isHittingGround = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsHittingGround(bool isPlayerHittingGround)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (_capsuleCollider.height + _capsuleCollider.radius) / 1.9f;
            _isHittingGround = hit.distance <= check;
        }
        return _isHittingGround;
    }

    public void Hurt(int damage) //FIXME
    {
        gameObject.SetActive(false); //Like this the List in CloningSystem works well!
    }

    public void Death() //FIXME
    {
        //StartCoroutine(Die());
        Destroy(gameObject); //If I do like this the List in CloningSystem won't work
    }
}
