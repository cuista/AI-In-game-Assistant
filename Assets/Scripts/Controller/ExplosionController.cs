using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ExplosionController : MonoBehaviour
{
    private static ExplosionController _explosionController;
    private static List<GameObject> _explosionsList;

    void Awake()
    {
        _explosionController = this;
        _explosionsList = new List<GameObject>();
    }

    //Instantiate explosion and destroy after its duration
    public static void MakeItBoom(GameObject explosionEffect, Transform origin)
    {
        GameObject explosion = Instantiate(explosionEffect, origin.position, origin.rotation);
        _explosionsList.Add(explosion);
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
    }

    public static void ClearExplosions()
    {
        foreach(GameObject explosion in _explosionsList)
        {
            if(explosion != null)
            {
                Destroy(explosion);
            }
        }
        _explosionsList.Clear();
    }
}
