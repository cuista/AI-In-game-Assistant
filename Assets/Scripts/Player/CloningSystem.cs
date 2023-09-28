using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

class PlayerRecord
{
    public Vector3 Position;
    public Quaternion Rotation;

    public PlayerRecord(Vector3 position, Quaternion rotation)
    {
        this.Position = position;
        this.Rotation = rotation;
    }
}

class ReplayClone
{
    public GameObject Clone;
    public List<PlayerRecord> PlayerRecordings;

    public ReplayClone(GameObject clone, List<PlayerRecord> playerRecordings)
    {
        this.Clone = clone;
        this.PlayerRecordings = playerRecordings;
    }
}

// Posso fare che ho tot. cloni e da quanto piazzo lo spawn registrano i movimenti, fino a che non spawno il clone
// ha più senso fare che con il tasto sinistro metto lo spawn e da quel momento inizia il timer dopo il quale appare il clone
// con il tasto destro potrei impostare i vari timer, a mano a mano che li sblocco (5sec, 10sec, 15sec, 20sec)

public class CloningSystem : MonoBehaviour
{
    [SerializeField] public GameObject clonePrefab;
    [SerializeField] public GameObject spawnClonePrefab;

    private List<ReplayClone> _replayClones;
    private GameObject _spawnPoint;
    private bool _spawnPointTimedOut;
    private ReplayClone _currentReplayClone;
    private int _clonesAvailable;

    // Start is called before the first frame update
    void Start()
    {
        _replayClones = new List<ReplayClone>();
        _spawnPoint = null;
        _spawnPointTimedOut = false;
        _currentReplayClone = null;
        _clonesAvailable = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if(_clonesAvailable > 0)
        {
            if(_spawnPointTimedOut) //SpawnPoint unused for 10 seconds
            {
                Destroy(_spawnPoint);
                _currentReplayClone = null;
                _spawnPointTimedOut = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if(_spawnPoint != null)
                {
                    Destroy(_spawnPoint);
                    _spawnPointTimedOut = false;
                }
                _spawnPoint = Instantiate(spawnClonePrefab) as GameObject;
                _spawnPoint.transform.position = transform.position;
                _currentReplayClone = new ReplayClone(null, new List<PlayerRecord>());
                StartCoroutine(SpawnPointUnused(_spawnPoint));
            }

            if (Input.GetMouseButtonUp(1) && _spawnPoint != null) //Spawn the clone and starting replaying player movements
            {
                Destroy(_spawnPoint);
                GameObject clone = Instantiate(clonePrefab, _currentReplayClone.PlayerRecordings[0].Position, _currentReplayClone.PlayerRecordings[0].Rotation);
                _currentReplayClone.Clone=clone;
                _replayClones.Add(_currentReplayClone);

                _currentReplayClone = null;
                _clonesAvailable--;
            }
        }

        if (_currentReplayClone != null) //If there's a clone ready to be placed, that is recording player movements
        {
            _currentReplayClone.PlayerRecordings.Add(new PlayerRecord(transform.position, transform.rotation));
        }

        if (_replayClones.Count>0) //If there's at least a clone in the scene
        {
            foreach (var replayClone in _replayClones)
            {
                replayClone.PlayerRecordings.Add(new PlayerRecord(transform.position, transform.rotation));
                replayClone.Clone.transform.SetPositionAndRotation(replayClone.PlayerRecordings[0].Position, replayClone.PlayerRecordings[0].Rotation);
                replayClone.PlayerRecordings.RemoveAt(0);
            }
        }
    }

    private IEnumerator SpawnPointUnused(GameObject spawn)
    {
        yield return new WaitForSeconds(10);

        if (spawn != null)
        {
            _spawnPointTimedOut = true;
        }
    }
}
