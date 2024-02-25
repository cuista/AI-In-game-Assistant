using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController_1 : MonoBehaviour
{
    private GameObject _player;
    private CharacterController _characterController;

    [SerializeField] private GameObject levelStartPoint;
    [SerializeField] private GameObject levelEndPoint;
    [SerializeField] private GameObject gemsContainer;
    [SerializeField] private GameObject triggerAILevelCompleted;

    private bool _allGemsTaken = false;
    private GameObject _levelEnd_Rays;

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
        PlaySoundtrack();

        _levelEnd_Rays = levelEndPoint.transform.GetChild(0).GetComponentInChildren<Animator>().gameObject;

        Managers.Inventory.ConsumeAll("Gems");
        levelEndPoint.GetComponent<BoxCollider>().enabled = false;
        _levelEnd_Rays.SetActive(false);
        triggerAILevelCompleted.SetActive(false);
        _player.GetComponent<PlayerCharacter>().SetGemsTotal(gemsContainer.transform.childCount);
    }

    private void FixedUpdate()
    {
        if (!_allGemsTaken && Managers.Inventory.GetItemCount("Gems") == gemsContainer.transform.childCount)
        {
            levelEndPoint.GetComponent<BoxCollider>().enabled = true;
            _levelEnd_Rays.SetActive(true);
            triggerAILevelCompleted.SetActive(true);
            _allGemsTaken = true;
        }
    }

    public void PlaySoundtrack()
    {
        AudioManager audioManager = DontDestroyOnLoadManager.GetAudioManager();
        audioManager.PlaySoundtrackLevel_1();
    }
}
