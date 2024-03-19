using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : SingletonBehaviour<GameSettings>
{
    public float MouseSensitivity { get; set; }
    public float AudioVolume { get; private set; }

    [SerializeField] AudioMixer _mixer;
    float _prevVolume;

    private void Start()
    {
        MouseSensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", 3);
        AudioVolume = PlayerPrefs.GetFloat("audio_volume", 3);
    }

    public void ChangeVolume(float vol)
    {
        AudioVolume = vol;
        PlayerPrefs.SetFloat("game_volume", AudioVolume);
    }

    public void ChangeSens(float sens)
    {
        MouseSensitivity = sens;
        PlayerPrefs.SetFloat("mouse_sensitivity", AudioVolume);
    }

    void Update()
    {
        if (!Mathf.Approximately(AudioVolume, _prevVolume))
        {
            _mixer.SetFloat("Master", AudioVolume);
        }

        _prevVolume = AudioVolume;
    }
}
