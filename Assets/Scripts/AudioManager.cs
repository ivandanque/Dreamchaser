using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    float volume = 1.0f;
    float sfxVolume = 1.0f;
    float musicVolume = 1.0f;

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }



    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip, volume);
    }

    void ChangeVolume(float newVolume, float newSFXVolume, float newMusicVolume)
    {
        volume = newVolume;
        sfxVolume = newSFXVolume;
        musicVolume = newMusicVolume;
    }

}
