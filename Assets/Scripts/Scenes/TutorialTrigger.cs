using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            CharacterController _characterController = other.GetComponent<CharacterController>();

            _characterController.enabled = false;
            other.transform.position = spawnPoint.transform.position;
            other.transform.rotation = spawnPoint.transform.rotation;
            _characterController.enabled = true;
        }
        else if (other.CompareTag("Clone") || other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<ICharacter>().Death();
        }
    }
}
