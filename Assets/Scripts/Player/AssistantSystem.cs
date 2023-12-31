using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssistantSystem : MonoBehaviour
{
    [SerializeField] private InworldAIController assistantController;
    private bool hintPressed;
    private bool mutePressed;

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

        _animatorHumanQuotes = humanQuotes.GetComponent<Animator>();

        humanQuotes.SetActive(true);
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameEvent.isPaused && assistantController != null)
        {
            if(Input.GetButtonDown("HintAssistant"))
            {
                hintPressed = true;
            }
            if(Input.GetButtonDown("MuteAssistant"))
            {
                mutePressed = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!GameEvent.isPaused && assistantController != null)
        {
            if (hintPressed)
            {
                switch ((int)Random.Range(0, 3))
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
                        quoteContent.text = "Tell me more!";
                        _audioSource.PlayOneShot(tellMeMore);
                        break;
                    case 4:
                        quoteContent.text = "Share more with me.";
                        _audioSource.PlayOneShot(shareMoreWithMe);
                        break;
                    case 5:
                        quoteContent.text = "I'm interested, continue!";
                        _audioSource.PlayOneShot(imInterestedContinue);
                        break;
                    case 6:
                    default:
                        quoteContent.text = "I'd love to hear more.";
                        _audioSource.PlayOneShot(idLoveToHearMore);
                        break;
                }

                _animatorHumanQuotes.SetTrigger("Open");
                if(assistantController.IsMuted())
                {
                    assistantController.Mute();
                }
                assistantController.HintTrigger();
                hintPressed = false;
            }
            else if (mutePressed)
            {
                switch ((int)Random.Range(0, 3))
                {
                    case 0:
                        quoteContent.text = assistantController.IsMuted() ? "Express yourself!" : "No more talking!";
                        _audioSource.PlayOneShot(assistantController.IsMuted() ? expressYourself : noMoreTalking);
                        break;
                    case 1:
                        quoteContent.text = assistantController.IsMuted() ? "Feel free to talk!" : "Silence please!";
                        _audioSource.PlayOneShot(assistantController.IsMuted() ? feelFreeToTalk : silencePlease);
                        break;
                    case 2:
                    default:
                        quoteContent.text = assistantController.IsMuted() ? "Talk freely!" : "Quiet down!";
                        _audioSource.PlayOneShot(assistantController.IsMuted() ? talkFreely : quietDown);
                        break;
                }

                _animatorHumanQuotes.SetTrigger("Open");
                assistantController.Mute();
                mutePressed = false;
            }
        }
    }
}
