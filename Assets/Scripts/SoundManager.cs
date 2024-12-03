using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SettingsUIController settingsUIController;

    private void Start()
    {
        // Set initial values from PlayerPrefs, otherwise default mixer setting.
        //MusicVolume set
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            Debug.Log("Music Volume was retrieved.");
            settingsUIController.SetMusicVolume(PlayerPrefs.GetInt("MusicVolume"));
        }
        else
        {
            settingsUIController.SetMusicVolume(Mathf.RoundToInt(settingsUIController.MapAudioMixerToSlider("MusicVolume")));
        }
        
        //SFXVolume set
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            settingsUIController.SetSFXVolume(PlayerPrefs.GetInt("SFXVolume"));
        }
        else
        {
            settingsUIController.SetSFXVolume(Mathf.RoundToInt(settingsUIController.MapAudioMixerToSlider("SFXVolume")));
        }
    }
}
