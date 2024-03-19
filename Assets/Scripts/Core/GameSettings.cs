using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : SingletonBehaviour<GameSettings>
{
    public float MouseSensitivity { get; set; }
    public float AudioVolume { get; private set; }

    [SerializeField] Slider _sensSlider;
    [SerializeField] Slider _audioSlider;

    [SerializeField] AudioMixer _mixer;
    float _prevVolume;

    private void Start()
    {
        MouseSensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", 0.5f);
        AudioVolume = PlayerPrefs.GetFloat("audio_volume", 1);

        _sensSlider.value = MouseSensitivity;
        _audioSlider.value = AudioVolume;
    }

    public void ChangeVolume(float vol)
    {
        AudioVolume = vol;

        if (AudioVolume < 1)
            AudioVolume = 0.001f;

        PlayerPrefs.SetFloat("game_volume", AudioVolume);
        // _mixer.SetFloat("Master", Mathf.Log10(AudioVolume / 100) * 20f);
    }

    public void ChangeSens(float sens)
    {
        MouseSensitivity = sens;
        PlayerPrefs.SetFloat("mouse_sensitivity", MouseSensitivity);
    }
}
