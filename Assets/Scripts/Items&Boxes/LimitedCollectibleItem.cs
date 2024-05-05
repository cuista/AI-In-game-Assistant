using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedCollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName;
    private Collider _collider;
    private Animator _animator;

    private const int CAPACITY = 8;
    private static readonly int maxItemCapacity = CAPACITY;

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
            if(Managers.Inventory.GetItemCount(itemName) < maxItemCapacity)
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
}
