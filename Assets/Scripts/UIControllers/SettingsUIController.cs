using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

class SettingsUIController : MonoBehaviour
{
    private VisualElement root;

    private SliderInt musicSlider;
    private SliderInt sfxSlider;
    public AudioMixer audioMixer;

    

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement backButton = root.Q<VisualElement>("BackButton");
        if (backButton != null)
        {
            backButton.RegisterCallback<ClickEvent>(ev => OnBackButtonClick());
        }
        musicSlider = root.Q<SliderInt>("MusicSlider");
        sfxSlider = root.Q<SliderInt>("SFXSlider");
        
        //Set sliders according to current level.
        sfxSlider.value = Mathf.RoundToInt(MapAudioMixerToSlider("SFXVolume"));
        musicSlider.value = Mathf.RoundToInt(MapAudioMixerToSlider("MusicVolume"));
        
        musicSlider.RegisterValueChangedCallback(evt => SetMusicVolume(evt.newValue));
        sfxSlider.RegisterValueChangedCallback(evt => SetSFXVolume(evt.newValue));
    }
    private void OnBackButtonClick()
    {
        //Save changes
        PlayerPrefs.SetInt("MusicVolume", musicSlider.value);
        PlayerPrefs.SetInt("SFXVolume", sfxSlider.value);
        
        GameManager.instance.SetPreviousState();
    }
    
    public void SetMusicVolume(int value)
    {
        float mappedValue = MapSliderToAudioMixer(value);
        audioMixer.SetFloat("MusicVolume", mappedValue);
    }

    public void SetSFXVolume(int value)
    {
        float mappedValue = MapSliderToAudioMixer(value);
        audioMixer.SetFloat("SFXVolume", mappedValue);
    }

    public float MapSliderToAudioMixer(int sliderValue)
    {
        // Custom mapping
        if (sliderValue == 0)
        {
            return -80f;
        }
        else if (sliderValue <= 10)
        {
            // Linear interpolation between 0 and 10
            return Mathf.Lerp(-80f, -20f, sliderValue / 10f);
        }
        else
        {
            // Linear interpolation between 10 and 100
            return Mathf.Lerp(-20f, 0f, (sliderValue - 10f) / 90f);
        }
    }

    public float MapAudioMixerToSlider(string parameterName)
    {
        float value;
        audioMixer.GetFloat(parameterName, out value);

        // Inverse custom mapping
        if (value <= -20f)
        {
            // Linear interpolation between -80 and -40
            return Mathf.Lerp(0f, 10f, (value + 80f) / 20f);
        }
        else
        {
            // Linear interpolation between -40 and 0
            return Mathf.Lerp(10f, 100f, (value + 20f) / 20f);
        }
    }

    private float GetAudioMixerValue(string parameterName)
    {
        float value;
        audioMixer.GetFloat(parameterName, out value);
        return value;
    }
}