using Inworld.Sample.RPM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController_0 : MonoBehaviour
{
    private GameObject _player;
    private CharacterController _characterController;

    [SerializeField] public GameObject levelStartPoint;
    [SerializeField] public GameObject levelEndPoint;

    public GameObject cutscene;

    [SerializeField] private GameObject moveTutorial;
    [SerializeField] private GameObject sprintTutorial;
    [SerializeField] private GameObject jumpTutorial;

    private void Awake()
    {
        Messenger.AddListener(GameEvent.CUTSCENE_STOPPED, AfterCutscene);
        BeforeCutscene();
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.CUTSCENE_STOPPED, AfterCutscene);
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = DontDestroyOnLoadManager.GetPlayer();
        _characterController = _player.GetComponent<CharacterController>();

        _characterController.enabled = false;
        _player.transform.position = levelStartPoint.transform.position;
        _player.transform.rotation = levelStartPoint.transform.rotation;
        _characterController.enabled = true;

        PlaySoundtrack();

        Managers.Inventory.ConsumeAll("Gems");
        _player.GetComponent<PlayerCharacter>().SetGemsTotal(-1);
    }

    public void PlaySoundtrack()
    {
        AudioManager audioManager = DontDestroyOnLoadManager.GetAudioManager();
        audioManager.PlaySoundtrackLevel_0();
    }

    public void BeforeCutscene()
    {
        GameEvent.isPaused = true;
        Cursor.lockState = CursorLockMode.Locked;

        DontDestroyOnLoadManager.GetMainCamera().SetActive(false);
        DontDestroyOnLoadManager.GetHUD().SetActive(false);
        DontDestroyOnLoadManager.GetSkipMessage().SetActive(true);

        Messenger.Broadcast(GameEvent.CUTSCENE_STARTED);

        DontDestroyOnLoadManager.GetAudioManager().StopCurrentSoundtrack();
    }

    public void AfterCutscene()
    {
        GameEvent.isPaused = false;

        cutscene.SetActive(false);
        DontDestroyOnLoadManager.GetMainCamera().SetActive(true);
        DontDestroyOnLoadManager.GetHUD().SetActive(true);
        DontDestroyOnLoadManager.GetSkipMessage().SetActive(false);

        Messenger.Broadcast(GameEvent.CUTSCENE_ENDED);
        _player.GetComponent<PlayerControllerRPM>().enabled = true;

        //if skip before cutscene makes level soundtrack playing
        AudioManager audioManager = DontDestroyOnLoadManager.GetAudioManager();
        if (!audioManager.isPlayingClip(audioManager.level0_soundtrack))
            audioManager.PlaySoundtrackLevel_0();

        StartCoroutine(ShowBasicTutorial());
    }

    private IEnumerator ShowBasicTutorial()
    {
        yield return new WaitForSeconds(0.5f);

        moveTutorial.SetActive(true);
        yield return new WaitForSeconds(3f);
        moveTutorial.SetActive(false);
        sprintTutorial.SetActive(true);
        yield return new WaitForSeconds(3f);
        sprintTutorial.SetActive(false);
        jumpTutorial.SetActive(true);
        yield return new WaitForSeconds(3f);
        jumpTutorial.SetActive(false);
    }
}
