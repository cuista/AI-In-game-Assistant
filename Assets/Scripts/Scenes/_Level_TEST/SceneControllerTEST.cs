using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControllerTEST : MonoBehaviour
{
    [SerializeField] private GameObject turretPrefab;
    private List<GameObject> _enemies;

    // Start is called before the first frame update
    void Start()
    {
        //ENEMIES
        _enemies = new List<GameObject>();
        /*
        AddEnemy(turretPrefab, new Vector3(-20, 0.54f, 45), Quaternion.Euler(0, 0, 0));
        AddEnemy(turretPrefab, new Vector3(7, 0.54f, 112f), Quaternion.Euler(0, -90, 0));
        AddEnemy(turretPrefab, new Vector3(-7, 7.04f, -20), Quaternion.Euler(0, 90, 0));
        */
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = _enemies.Count - 1; i >= 0; --i)
        {
            if (_enemies[i] == null)
            {
                _enemies.RemoveAt(i);
                Messenger.Broadcast(GameEvent.ENEMY_KILLED);
            }
        }
    }

    private void AddEnemy(GameObject enemyPrefab, Vector3 position, Quaternion rotation)
    {
        _enemies.Add(Instantiate(enemyPrefab, position, rotation));
    }

}
