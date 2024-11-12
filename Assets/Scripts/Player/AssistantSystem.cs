using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssistantSystem : MonoBehaviour
{
    [SerializeField] private InworldAIController assistantInworldAIController;
    [SerializeField] private AssistantOpenAIController assistantOpenAIController;
    [SerializeField] private VisionOpenAIController visionOpenAIController;
    private bool hintPressed;
    private bool mutePressed;
    private bool visionPressed;

    [SerializeField] private GameObject humanQuotes;
    [SerializeField] private TMP_Text quoteContent;

    private Animator _animatorHumanQuotes;

    private AudioSource _audioSource;

    [SerializeField] private AudioClip goOn;
    [SerializeField] private AudioClip keepTalking;
    [SerializeField] private AudioClip iWantToHearMore;
    [SerializeField] private AudioClip tellMeMore;
    [SerializeField] private AudioClip shareMoreWithMe;
    [SerializeField] private AudioClip imInterestedContinue;
    [SerializeField] private AudioClip idLoveToHearMore;

    [SerializeField] private AudioClip noMoreTalking;
    [SerializeField] private AudioClip silencePlease;
    [SerializeField] private AudioClip quietDown;

    [SerializeField] private AudioClip expressYourself;
    [SerializeField] private AudioClip feelFreeToTalk;
    [SerializeField] private AudioClip talkFreely;

    // Start is called before the first frame update
    void Start()
    {
        hintPressed = false;
        mutePressed = false;
        visionPressed = false;

        _animatorHumanQuotes = humanQuotes.GetComponent<Animator>();

        humanQuotes.SetActive(true);
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameEvent.isPaused && assistantInworldAIController != null)
        {
            if(Input.GetButtonDown("HintAssistant"))
            {
                hintPressed = true;
            }
            if(Input.GetButtonDown("MuteAssistant"))
            {
                mutePressed = true;
            }
            if (Input.GetButtonDown("VisionAssistant"))
            {
                visionPressed = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!GameEvent.isPaused && assistantInworldAIController != null)
        {
            if (hintPressed)
            {
                switch ((int)Random.Range(0, 4))
                {
                    case 0:
                        quoteContent.text = "Go on, I'm listening!";
                        _audioSource.PlayOneShot(goOn);
                        break;
                    case 1:
                        quoteContent.text = "Keep talking!";
                        _audioSource.PlayOneShot(keepTalking);
                        break;
                    case 2:
                        quoteContent.text = "I want to hear more.";
                        _audioSource.PlayOneShot(iWantToHearMore);
                        break;
                    case 3:
                    default:
                        quoteContent.text = "I'd love to hear more.";
                        _audioSource.PlayOneShot(idLoveToHearMore);
                        break;
                }

                _animatorHumanQuotes.SetTrigger("Open");
                if(assistantInworldAIController.IsMuted())
                {
                    assistantInworldAIController.Mute();
                }
                assistantInworldAIController.HintTrigger();
                assistantOpenAIController.HintTrigger();
                hintPressed = false;
            }
            else if (mutePressed)
            {
                switch ((int)Random.Range(0, 3))
                {
                    case 0:
                        quoteContent.text = assistantInworldAIController.IsMuted() ? "Express yourself!" : "No more talking!";
                        _audioSource.PlayOneShot(assistantInworldAIController.IsMuted() ? expressYourself : noMoreTalking);
                        break;
                    case 1:
                        quoteContent.text = assistantInworldAIController.IsMuted() ? "Feel free to talk!" : "Silence please!";
                        _audioSource.PlayOneShot(assistantInworldAIController.IsMuted() ? feelFreeToTalk : silencePlease);
                        break;
                    case 2:
                    default:
                        quoteContent.text = assistantInworldAIController.IsMuted() ? "Talk freely!" : "Quiet down!";
                        _audioSource.PlayOneShot(assistantInworldAIController.IsMuted() ? talkFreely : quietDown);
                        break;
                }

                _animatorHumanQuotes.SetTrigger("Open");
                assistantInworldAIController.Mute();
                mutePressed = false;
            }
            else if (visionPressed)
            {
                switch ((int)Random.Range(0, 3))
                {
                    case 0:
                        quoteContent.text = "Tell me more!";
                        _audioSource.PlayOneShot(tellMeMore);
                        break;
                    case 1:
                        quoteContent.text = "Share more with me.";
                        _audioSource.PlayOneShot(shareMoreWithMe);
                        break;
                    case 2:
                    default:
                        quoteContent.text = "I'm interested, continue!";
                        _audioSource.PlayOneShot(imInterestedContinue);
                        break;
                }

                _animatorHumanQuotes.SetTrigger("Open");
                if (assistantInworldAIController.IsMuted())
                {
                    assistantInworldAIController.Mute();
                }
                visionOpenAIController.VisionTrigger();
                visionPressed = false;
            }
        }
    }
}
