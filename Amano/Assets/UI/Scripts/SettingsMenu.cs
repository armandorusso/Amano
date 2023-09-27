using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Slider = UnityEngine.UI.Slider;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] public Slider MusicSlider;
    [SerializeField] public AudioMixer AudioMixer;

    public void OnMusicSliderChanged()
    {
        var sliderValue = MusicSlider.value;
        
        // Audio Mixer changes the volume logarithmically
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }
}
