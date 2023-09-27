using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemName;

    //add item
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>())
        {
            Debug.Log("Consumed: " + itemName);
            Managers.Inventory.AddItem(itemName);
            Destroy(this.gameObject);
        }
    }
}
