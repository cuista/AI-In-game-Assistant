using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController_0 : MonoBehaviour
{
    private GameObject _player;
    private CharacterController _characterController;

    [SerializeField] private GameObject levelStartPoint;
    [SerializeField] private GameObject levelTutorialPoint;
    [SerializeField] private GameObject levelStoryPoint;

    public GameObject cutscene;
    [SerializeField] private GameObject tutorialText;
    [SerializeField] private GameObject levelsText;

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

        SetChildernsActive(levelTutorialPoint, false);
        SetChildernsActive(levelStoryPoint, false);

        PlaySoundtrack();

        Managers.Inventory.ConsumeAll("Gems");
        _player.GetComponent<PlayerCharacter>().SetGemsTotal(-1);
    }

    private void SetChildernsActive(GameObject go, bool isActive)
    {
        for(int i=0; i < go.transform.childCount; i++)
        {
            go.transform.GetChild(i).gameObject.SetActive(isActive);
        }
    }

    public void PlaySoundtrack()
    {
        AudioManager audioManager = DontDestroyOnLoadManager.GetAudioManager();
        audioManager.PlaySoundtrackLevel_0();
    }

    public void PlayCutscenetrack()
    {
        AudioManager audioManager = DontDestroyOnLoadManager.GetAudioManager();
        audioManager.PlaySoundtrackCutscene();
    }

    public void BeforeCutscene()
    {
        GameEvent.isPaused = true;
        Cursor.lockState = CursorLockMode.Locked;

        DontDestroyOnLoadManager.GetMainCamera().SetActive(false);
        DontDestroyOnLoadManager.GetHUD().SetActive(false);
        DontDestroyOnLoadManager.GetSkipMessage().SetActive(true);

        tutorialText.SetActive(false);
        levelsText.SetActive(false);

        DontDestroyOnLoadManager.GetPlayer().GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

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

        tutorialText.SetActive(true);
        levelsText.SetActive(true);

        DontDestroyOnLoadManager.GetPlayer().GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;

        Messenger.Broadcast(GameEvent.CUTSCENE_ENDED);

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

        SetChildernsActive(levelTutorialPoint, true);
        yield return new WaitForSeconds(0.5f);
        SetChildernsActive(levelStoryPoint, true);
    }
}
