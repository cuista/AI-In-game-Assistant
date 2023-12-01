using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController_2 : MonoBehaviour
{
    private GameObject _player;
    private CharacterController _characterController;

    [SerializeField] private GameObject levelStartPoint;
    [SerializeField] private GameObject levelEndPoint;

    private void Awake()
    {
        _player = DontDestroyOnLoadManager.GetPlayer();
        _characterController = _player.GetComponent<CharacterController>();

        _characterController.enabled = false;
        _player.transform.position = levelStartPoint.transform.position;
        _player.transform.rotation = levelStartPoint.transform.rotation;
        _characterController.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
    }

    public void PlaySoundtrack()
    {
        AudioManager audioManager = DontDestroyOnLoadManager.GetAudioManager();
        if (!audioManager.isPlayingClip(audioManager.level2_soundtrack))
            audioManager.PlaySoundtrackLevel_0();
    }
}
