using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _soundtrackSource;

    public AudioClip intro_init;
    public AudioClip menu_soundtrack;
    public AudioClip gameOver_soundtrack;
    public AudioClip level0_soundtrack;
    public AudioClip level1_soundtrack;
    public AudioClip level2_soundtrack;
    public AudioClip bossFight_soundtrack;
    public AudioClip credits_soundtrack;
    public AudioClip VR_Mode_soundtrack;

    public float soundVolume
    {
        get { return AudioListener.volume; }
        set { AudioListener.volume = value; }
    }

    public bool soundMute
    {
        get { return AudioListener.pause; }
        set { AudioListener.pause = value; }
    }

    public float musicVolume
    {
        get
        {
            return _soundtrackSource.volume;
        }
        set
        {
            if (_soundtrackSource != null)
            {
                _soundtrackSource.volume = value;
            }
        }
    }

    public bool musicMute
    {
        get
        {
            if (_soundtrackSource != null)
            {
                return _soundtrackSource.mute;
            }
            return false;
        }
        set
        {
            if (_soundtrackSource != null)
            {
                _soundtrackSource.mute = value;
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        _soundtrackSource.ignoreListenerVolume = true;
        _soundtrackSource.ignoreListenerPause = true;
        soundVolume = 1f;
        musicVolume = 0.5f;
        _soundtrackSource.PlayOneShot(intro_init);
    }


    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    public void PlaySoundtrackMenuClip()
    {
        _soundtrackSource.clip = menu_soundtrack;
        _soundtrackSource.Play();
    }

    public void PlaySoundtrackGameOver()
    {
        _soundtrackSource.clip = gameOver_soundtrack;
        _soundtrackSource.Play();
    }

    public void PlaySoundtrackLevel_0()
    {
        _soundtrackSource.clip = level0_soundtrack;
        _soundtrackSource.Play();
    }

    public void PlaySoundtrackLevel_1()
    {
        _soundtrackSource.clip = level1_soundtrack;
        _soundtrackSource.Play();
    }

    public void PlaySoundtrackLevel_2()
    {
        _soundtrackSource.clip = level2_soundtrack;
        _soundtrackSource.Play();
    }

    public void PlaySoundtrackBossFight()
    {
        _soundtrackSource.clip = bossFight_soundtrack;
        _soundtrackSource.Play();
    }

    public void PlaySoundtrackCredits()
    {
        _soundtrackSource.clip = credits_soundtrack;
        _soundtrackSource.Play();
    }

    public void PlaySoundtrackVRMode()
    {
        _soundtrackSource.clip = VR_Mode_soundtrack;
        _soundtrackSource.Play();
    }

    public void StopCurrentSoundtrack()
    {
        _soundtrackSource.Stop();
    }

    public bool isPlayingClip(AudioClip audio)
    {
        return (_audioSource.clip == audio || _soundtrackSource.clip == audio) ? true : false;
    }

    public float GetCreditsClipLength()
    {
        return credits_soundtrack.length;
    }
}

