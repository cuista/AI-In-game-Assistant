using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController_1 : MonoBehaviour
{
    private GameObject _player;

    [SerializeField] public GameObject levelStartPoint;
    [SerializeField] public GameObject levelEndPoint;

    [SerializeField] private GameObject turretPrefab;
    private List<GameObject> _enemies;

    private void Awake()
    {
        _player = DontDestroyOnLoadManager.GetPlayer();
        _player.transform.position = levelStartPoint.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ENEMIES
        _enemies = new List<GameObject>();
        AddEnemy(turretPrefab, new Vector3(20, 1.2f, 113), Quaternion.Euler(0, 180, 0));
        AddEnemy(turretPrefab, new Vector3(20, 0.54f, 185), Quaternion.Euler(0, 180, 0));
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

    public void PlaySoundtrack()
    {
        AudioManager audioManager = DontDestroyOnLoadManager.GetAudioManager();
        if (!audioManager.isPlayingClip(audioManager.level0_soundtrack))
            audioManager.PlaySoundtrackLevel_0();
    }
}
