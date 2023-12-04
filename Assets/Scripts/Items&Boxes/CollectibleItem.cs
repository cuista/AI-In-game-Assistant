using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName;
    private Collider _collider;
    private Animator _animator;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _animator = GetComponentInChildren<Animator>();
    }

    //add item
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>())
        {
            Debug.Log("Consumed: " + itemName); //Debug

            _collider.enabled = false;
            Managers.Inventory.AddItem(itemName);
            if (_animator != null)
            {
                _animator.SetTrigger("TakeItem"); //The script ItemTaken will destroy the item when animation will finish
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
