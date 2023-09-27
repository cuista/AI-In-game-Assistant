using System;
using System.Collections;
using System.Collections.Generic;
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

public class CloningSystem : MonoBehaviour
{
    [SerializeField] private GameObject _clone;
    [SerializeField] private GameObject _spawnClone;
    private Boolean _isRecordingPlayer;

    private List<PlayerRecord> playerRecordings;
    // Start is called before the first frame update
    void Start()
    {
        _clone.SetActive(false);
        _spawnClone.SetActive(false);
        _isRecordingPlayer = false;
        playerRecordings = new List<PlayerRecord>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !_clone.activeInHierarchy)
        {
            //Clone will spawn in this position and start registration
            playerRecordings.Clear();
            _isRecordingPlayer = true;

            _spawnClone.transform.position = transform.position;
            _spawnClone.SetActive(true);
        }
        else if (Input.GetMouseButtonDown(1) && !_clone.activeInHierarchy)
        {
            //Clone spawn and starting replaying player movements
            _clone.SetActive(true);
            _clone.transform.position = playerRecordings[0].Position;
            _clone.transform.rotation = playerRecordings[0].Rotation;
            playerRecordings.RemoveAt(0);

            _spawnClone.SetActive(false);
        }

        if(_isRecordingPlayer)
        {
            playerRecordings.Add(new PlayerRecord(transform.position, transform.rotation));
        }

        if(_clone.activeInHierarchy)
        {
            _clone.transform.position = playerRecordings[0].Position;
            _clone.transform.rotation = playerRecordings[0].Rotation;
            playerRecordings.RemoveAt(0);
        }
    }
}
