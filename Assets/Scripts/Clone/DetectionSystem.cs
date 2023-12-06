using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionSystem : MonoBehaviour
{
    public Color sphereColor = Color.yellow;
    public float detectionRadius = 10f;
    public Color linecastColor = Color.red;
    public float detectionAngle = 90f;

    private void OnDrawGizmos() //DEBUG
    {
        Gizmos.color = sphereColor;

        // Disegna l'OverlapSphere con il raggio specificato
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            GameObject hitOverlapSphere = hitCollider.transform.gameObject;
            // Verifica se il giocatore è nel raggio
            if (hitOverlapSphere.CompareTag("Player"))
            {
                float fovAngle = Vector3.Angle(transform.forward, hitOverlapSphere.transform.position - transform.position);
                
                RaycastHit hitLinecast;
                Physics.Linecast(transform.position, hitOverlapSphere.transform.position, out hitLinecast);

                // Esegui il Linecast e verifica se colpisce il giocatore
                if (Mathf.Abs(fovAngle) <= detectionAngle / 2 && hitLinecast.transform.gameObject.CompareTag("Player"))
                {
                    hitOverlapSphere.GetComponent<PlayerCharacter>().Death();
                    linecastColor = Color.green; // Cambia il colore del Linecast a verde
                }
                else
                {
                    linecastColor = Color.red; // Mantieni il colore del Linecast a rosso
                }

                //DEBUG Draw Linecast
                Debug.DrawRay(transform.position, (hitOverlapSphere.transform.position - transform.position).normalized * detectionRadius, linecastColor);
            }
        }
    }
}
