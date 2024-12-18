using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AssistantSystem : MonoBehaviour
{
    [SerializeField] private AssistantOpenAIController assistantOpenAIController;
    [SerializeField] private VisionOpenAIController visionOpenAIController;
    private bool hintPressed;
    private bool mutePressed;
    private float lastButtonPressedTime;

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
        lastButtonPressedTime = Time.time;

        _animatorHumanQuotes = humanQuotes.GetComponent<Animator>();

        humanQuotes.SetActive(true);
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameEvent.isPaused && (Time.time - lastButtonPressedTime) > 2.5f)
        {
            if(Input.GetButtonDown("HintAssistant"))
            {
                hintPressed = true;
                lastButtonPressedTime = Time.time;
            }
            if(Input.GetButtonDown("MuteAssistant"))
            {
                mutePressed = true;
                lastButtonPressedTime = Time.time;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!GameEvent.isPaused)
        {
            if (hintPressed)
            {
                switch ((int)Random.Range(0, 4))
                {
                    case 0:
                        quoteContent.text = "I want to hear more.";
                        _audioSource.PlayOneShot(iWantToHearMore);
                        break;
                    case 1:
                        quoteContent.text = "I'd love to hear more.";
                        _audioSource.PlayOneShot(idLoveToHearMore);
                        break;
                    case 2:
                        quoteContent.text = "Tell me more!";
                        _audioSource.PlayOneShot(tellMeMore);
                        break;
                    case 3:
                        quoteContent.text = "Share more with me.";
                        _audioSource.PlayOneShot(shareMoreWithMe);
                        break;
                    case 4:
                    default:
                        quoteContent.text = "Keep talking!";
                        _audioSource.PlayOneShot(keepTalking);
                        break;
                }

                _animatorHumanQuotes.SetTrigger("Open");

                if (assistantOpenAIController != null && !assistantOpenAIController.IsMuted())
                {
                    assistantOpenAIController.HintTrigger();
                }
                
                hintPressed = false;
            }
            else if (mutePressed)
            {
                if(assistantOpenAIController != null)
                {
                    switch ((int)Random.Range(0, 3))
                    {
                        case 0:
                            quoteContent.text = assistantOpenAIController.IsMuted() ? "Express yourself!" : "No more talking!";
                            _audioSource.PlayOneShot(assistantOpenAIController.IsMuted() ? expressYourself : noMoreTalking);
                            break;
                        case 1:
                            quoteContent.text = assistantOpenAIController.IsMuted() ? "Feel free to talk!" : "Silence please!";
                            _audioSource.PlayOneShot(assistantOpenAIController.IsMuted() ? feelFreeToTalk : silencePlease);
                            break;
                        case 2:
                        default:
                            quoteContent.text = assistantOpenAIController.IsMuted() ? "Talk freely!" : "Quiet down!";
                            _audioSource.PlayOneShot(assistantOpenAIController.IsMuted() ? talkFreely : quietDown);
                            break;
                    }

                    assistantOpenAIController.Mute();
                }

                _animatorHumanQuotes.SetTrigger("Open");
                mutePressed = false;
            }
        }
    }
}
