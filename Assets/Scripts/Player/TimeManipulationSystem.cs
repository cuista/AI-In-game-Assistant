using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TimeManipulationSystem : MonoBehaviour
{
    public float timeAcceleration = 3f;
    public float timeDeceleration = 0.5f;

    [SerializeField] private Volume _VHSEffect;
    [SerializeField] private Volume _SlowMotionEffect;

    public float maxDuration = 5f;
    public float recoveryTime = 3f;

    [SerializeField] public Slider timeAccelerationSlider;
    private bool _canAccelerateTime = true;
    private float accelerationDuration;

    [SerializeField] public Slider timeDecelerationSlider;
    private bool _canDecelerateTime = true;
    private float decelerationDuration;

    [SerializeField] public AudioSource timeManipulationAudioSource;

    [SerializeField] private AudioClip timeAccelerationSound;
    [SerializeField] private AudioClip accelerationReadySound;

    [SerializeField] private AudioClip timeDecelerationSound;
    [SerializeField] private AudioClip decelerationReadySound;

    // Start is called before the first frame update
    void Start()
    {
        accelerationDuration = maxDuration;
        decelerationDuration = maxDuration;

        _VHSEffect.enabled = false;
        _SlowMotionEffect.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameEvent.isPaused)
        {
            //Time acceleration ability
            if (Input.GetButtonUp("TimeAcceleration"))
            {
                if(_canAccelerateTime)
                {
                    decelerationDuration = 0;
                    StartCoroutine(UseTimeAcceleration());
                }
                else
                {
                    accelerationDuration = 0;
                }
            }

            //Time deceleration ability
            if (Input.GetButtonDown("TimeDeceleration"))
            {
                if (_canDecelerateTime)
                {
                    accelerationDuration = 0;
                    StartCoroutine(UseTimeDeceleration());
                }
                else
                {
                    decelerationDuration = 0;
                }
            }
        }
    }

    private IEnumerator UseTimeAcceleration()
    {
        accelerationDuration = maxDuration;

        _canAccelerateTime = false;

        // Accelerate time
        Time.timeScale = timeAcceleration;
        _VHSEffect.enabled = true;

        timeManipulationAudioSource.clip = timeAccelerationSound;
        timeManipulationAudioSource.Play();

        float totalTime = 0;

        accelerationDuration *= timeAcceleration;

        while (totalTime <= accelerationDuration)
        {
            if (!GameEvent.isPaused)
            {
                Time.timeScale = timeAcceleration;
                timeAccelerationSlider.value = 1 - (totalTime / accelerationDuration);
                totalTime += Time.deltaTime;
            }
            yield return null;
        }

        Time.timeScale = 1.0f;
        _VHSEffect.enabled = false;
        timeManipulationAudioSource.Stop();

        // Recharge time acceleration
        totalTime = 0;
        while (totalTime <= recoveryTime)
        {
            if (!GameEvent.isPaused)
            {
                timeAccelerationSlider.value = totalTime / recoveryTime;
                totalTime += Time.deltaTime;
            }
            yield return null;
        }

        timeManipulationAudioSource.Stop();
        timeManipulationAudioSource.PlayOneShot(accelerationReadySound);

        _canAccelerateTime = true;
    }

    private IEnumerator UseTimeDeceleration()
    {
        decelerationDuration = maxDuration;

        _canDecelerateTime = false;

        // Decelerate time
        Time.timeScale = timeDeceleration;
        _SlowMotionEffect.enabled = true;

        timeManipulationAudioSource.clip = timeDecelerationSound;
        timeManipulationAudioSource.Play();

        float totalTime = 0;

        decelerationDuration *= timeDeceleration;

        while (totalTime <= decelerationDuration)
        {
            if (!GameEvent.isPaused)
            {
                Time.timeScale = timeDeceleration;
                timeDecelerationSlider.value = 1 - (totalTime / decelerationDuration);
                totalTime += Time.deltaTime;
            }
            yield return null;
        }

        Time.timeScale = 1.0f;
        _SlowMotionEffect.enabled = false;
        timeManipulationAudioSource.Stop();

        // Recharge time deceleration
        totalTime = 0;
        while (totalTime <= recoveryTime)
        {
            if (!GameEvent.isPaused)
            {
                timeDecelerationSlider.value = totalTime / recoveryTime;
                totalTime += Time.deltaTime;
            }
            yield return null;
        }

        timeManipulationAudioSource.Stop();
        timeManipulationAudioSource.PlayOneShot(decelerationReadySound);

        _canDecelerateTime = true;
    }
}
