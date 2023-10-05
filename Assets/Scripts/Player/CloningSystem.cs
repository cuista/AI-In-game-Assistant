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

public class CloningSystem : MonoBehaviour
{
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private GameObject spawnClonePrefab;

    private List<ReplayClone> _replayClones;
    private GameObject _spawnPoint;
    private bool _spawnPointTimedOut;
    private ReplayClone _currentReplayClone;
    private int _clonesAvailable;

    private bool fire1Pressed;
    private bool fire2Pressed;

    // Start is called before the first frame update
    void Start()
    {
        _replayClones = new List<ReplayClone>();
        _spawnPoint = null;
        _spawnPointTimedOut = false;
        _currentReplayClone = null;
        _clonesAvailable = 8; //Number of max. clones that can be placed

        fire1Pressed = false;
        fire2Pressed = false;
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

            fire1Pressed = Input.GetButtonUp("Fire1") || fire1Pressed;
            fire2Pressed = (Input.GetButtonUp("Fire2") && _spawnPoint != null) || fire2Pressed;
        }
    }

    private void FixedUpdate()
    {
        if(fire1Pressed)
        {
            if (_spawnPoint != null)
            {
                Destroy(_spawnPoint);
                _spawnPointTimedOut = false;
            }
            _spawnPoint = Instantiate(spawnClonePrefab) as GameObject;
            _spawnPoint.transform.position = transform.position;
            _currentReplayClone = new ReplayClone(null, new List<PlayerRecord>());
            StartCoroutine(SpawnPointUnused(_spawnPoint));

            transform.GetComponent<PlayerCharacter>().DisableUICountdown(); //UI temporary

            fire1Pressed = false;
        }

        if (fire2Pressed) //Spawn the clone and starting replaying player movements
        {
            Destroy(_spawnPoint);
            GameObject clone = Instantiate(clonePrefab, _currentReplayClone.PlayerRecordings[0].Position, _currentReplayClone.PlayerRecordings[0].Rotation);
            _currentReplayClone.Clone = clone;
            _replayClones.Add(_currentReplayClone);

            _currentReplayClone = null;
            _clonesAvailable--;

            transform.GetComponent<PlayerCharacter>().DisableUICountdown(); //UI temporary

            fire2Pressed = false;
        }

        //If there's a clone ready to be placed, a not null clone ready that is recording player movements
        _currentReplayClone?.PlayerRecordings.Add(new PlayerRecord(transform.position, transform.rotation));

        if (_replayClones.Count > 0) //If there's at least a clone in the scene
        {
            foreach (var replayClone in _replayClones)
            {
                replayClone.PlayerRecordings.Add(new PlayerRecord(transform.position, transform.rotation));
                Debug.Log("pos + rot" + replayClone.PlayerRecordings[0].Position + "; " + replayClone.PlayerRecordings[0].Rotation);
                replayClone.Clone.transform.SetPositionAndRotation(replayClone.PlayerRecordings[0].Position, replayClone.PlayerRecordings[0].Rotation);
                replayClone.PlayerRecordings.RemoveAt(0);
            }
        }

    }

    private IEnumerator SpawnPointUnused(GameObject spawn) //if a spawn point is unused it will be remove after x seconds
    {
        StartCoroutine(transform.GetComponent<PlayerCharacter>().UICountdown(20)); //UI temporary
        yield return new WaitForSeconds(20);

        if (spawn != null)
        {
            _spawnPointTimedOut = true;
        }
    }
}
