using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone") || other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<ICharacter>().Death();
        }
    }
}
