using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController_Tutorial : MonoBehaviour
{
    private GameObject _player;
    private CharacterController _characterController;

    [SerializeField] private GameObject levelStartPoint;
    [SerializeField] private GameObject levelEndPoint;
    [SerializeField] private GameObject gemsContainer;
    [SerializeField] private GameObject triggerAILevelCompleted;

    [SerializeField] private TargetObject emersionPlatform1;
    private bool _isActivatedEmersionPlatform1 = false;

    [SerializeField] private TargetObject emersionPlatform2;
    private bool _isTriggeredEmersionPlatform2 = false;

    private bool _allGemsTaken = false;
    private GameObject _levelEnd_Rays;

    [SerializeField] private GameObject tutorialCanvas;

    [SerializeField] private GameObject moveTutorial;
    [SerializeField] private GameObject sprintTutorial;
    [SerializeField] private GameObject jumpTutorial;
    [SerializeField] private GameObject meleeTutorial;
    [SerializeField] private GameObject potionsTutorial;
    [SerializeField] private GameObject cloneRechargeTutorial;
    [SerializeField] private GameObject echoHintTutorial;
    [SerializeField] private GameObject echoMuteTutorial;
    [SerializeField] private GameObject spawnPointTutorial;
    [SerializeField] private GameObject spawnCloneTutorial;
    [SerializeField] private GameObject timeAccelerationTutorial;
    [SerializeField] private GameObject timeDecelerationTutorial;
    [SerializeField] private GameObject leverTutorial;
    [SerializeField] private GameObject finishLevelTutorial;

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
        Managers.Inventory.ConsumeAll("CloneRecharge");
        levelEndPoint.GetComponent<BoxCollider>().enabled = false;
        _levelEnd_Rays.SetActive(false);
        triggerAILevelCompleted.SetActive(false);
        _player.GetComponent<PlayerCharacter>().SetGemsTotal(gemsContainer.transform.childCount);

        StartCoroutine(ShowBasicTutorial());
    }

    private void FixedUpdate()
    {
        if (!_isActivatedEmersionPlatform1 && Managers.Inventory.GetItemCount("Gems") >= 12)
        {
            emersionPlatform1.Activate();
            _isActivatedEmersionPlatform1 = true;
            StartCoroutine(ShowJumpTutorial());
        }

        if (_isTriggeredEmersionPlatform2)
        {
            emersionPlatform2.Activate();
            _isTriggeredEmersionPlatform2 = false;
        }

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
        audioManager.PlaySoundtrackTutorial();
    }

    private IEnumerator ShowBasicTutorial()
    {
        yield return new WaitForSeconds(2f);

        disableOtherTutorials();

        moveTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        moveTutorial.SetActive(false);
        sprintTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        sprintTutorial.SetActive(false);
    }

    public IEnumerator ShowJumpTutorial()
    {
        disableOtherTutorials();

        jumpTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        jumpTutorial.SetActive(false);
    }

    public void ShowItemsTutorial() //Trigger01
    {
        StartCoroutine(ShowCollectiblesTutorial());
    }

    private IEnumerator ShowCollectiblesTutorial()
    {
        yield return new WaitForSeconds(0.5f);

        disableOtherTutorials();

        meleeTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        meleeTutorial.SetActive(false);
        potionsTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        potionsTutorial.SetActive(false);
        cloneRechargeTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        cloneRechargeTutorial.SetActive(false);

        _isTriggeredEmersionPlatform2 = true;
    }

    public void ShowEchoTutorial() //Trigger02
    {
        StartCoroutine(ShowAiAssistantTutorial());
    }

    private IEnumerator ShowAiAssistantTutorial()
    {
        yield return new WaitForSeconds(0.5f);

        disableOtherTutorials();

        echoHintTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        echoHintTutorial.SetActive(false);
        echoMuteTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        echoMuteTutorial.SetActive(false);
    }

    public void ShowClonesTutorial() //Trigger03
    {
        StartCoroutine(ShowCloneSystemTutorial());
    }

    private IEnumerator ShowCloneSystemTutorial()
    {
        yield return new WaitForSeconds(0.5f);

        disableOtherTutorials();

        spawnPointTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        spawnPointTutorial.SetActive(false);
        spawnCloneTutorial.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        spawnCloneTutorial.SetActive(false);
    }

    public void ShowTimeTutorial() //Trigger04
    {
        StartCoroutine(ShowTimeManipulationTutorial());
    }

    private IEnumerator ShowTimeManipulationTutorial()
    {
        disableOtherTutorials();

        timeAccelerationTutorial.SetActive(true);
        yield return new WaitForSeconds(4f);
        timeAccelerationTutorial.SetActive(false);
        timeDecelerationTutorial.SetActive(true);
        yield return new WaitForSeconds(4f);
        timeDecelerationTutorial.SetActive(false);
    }

    public void ShowFinishLevelTutorial() //Trigger05
    {
        StartCoroutine(ShowLevelCollectGemsTutorial());
    }

    private IEnumerator ShowLevelCollectGemsTutorial()
    {
        disableOtherTutorials();

        leverTutorial.SetActive(true);
        yield return new WaitForSeconds(4f);
        leverTutorial.SetActive(false);
        finishLevelTutorial.SetActive(true);
        yield return new WaitForSeconds(4f);
        finishLevelTutorial.SetActive(false);
    }

    private void disableOtherTutorials()
    {
        for (int i = 0; i < tutorialCanvas.transform.childCount; i++)
        {
            tutorialCanvas.transform.GetChild(i).gameObject.SetActive(false);
        }
    }    

}
