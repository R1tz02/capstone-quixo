using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    public Slider masterSlider, musicSlider, sfxSlider;
    float masterLevel, musicLevel, sfxLevel;

    private void Start()
    {
        audioMixer.GetFloat("masterVolume", out float masterVolume);
        masterLevel = Mathf.Pow(10f, masterVolume / 20);
        masterSlider.value = masterLevel;

        audioMixer.GetFloat("soundFXVolume", out float soundFXVolume);
        sfxLevel = Mathf.Pow(10f, soundFXVolume / 20);
        sfxSlider.value = sfxLevel;

        audioMixer.GetFloat("musicVolume", out float musicVolume);
        musicLevel = Mathf.Pow(10f, musicVolume / 20);
        musicSlider.value = musicLevel;
    }

    public void SetMasterVolume(float level)
    {
        // this should be working
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
    }
}
