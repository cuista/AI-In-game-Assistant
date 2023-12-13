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

        // Draw the OverlapSphere with detectionRadius
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    void FixedUpdate()
    {
        if (!GameEvent.isPaused)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
            foreach (var hitCollider in hitColliders)
            {
                GameObject hitOverlapSphere = hitCollider.transform.gameObject;
                // Check if player is within the radius
                if (hitOverlapSphere.CompareTag("Player"))
                {
                    float fovAngle = Vector3.Angle(transform.forward, hitOverlapSphere.transform.position - transform.position);
                
                    RaycastHit hitLinecast;
                    Physics.Linecast(transform.position, hitOverlapSphere.transform.position, out hitLinecast);

                    // Do Linecast and check if hit the player
                    if (Mathf.Abs(fovAngle) <= detectionAngle / 2 && hitLinecast.transform.gameObject.CompareTag("Player"))
                    {
                        hitOverlapSphere.GetComponent<PlayerCharacter>().Death();
                        linecastColor = Color.green; // Change Linecast color to green
                    }
                    else
                    {
                        linecastColor = Color.red; // Keep Linecast color to red
                    }

                    //DEBUG Draw Linecast
                    Debug.DrawRay(transform.position, (hitOverlapSphere.transform.position - transform.position).normalized * detectionRadius, linecastColor);
                }
            }
        }
    }
}
