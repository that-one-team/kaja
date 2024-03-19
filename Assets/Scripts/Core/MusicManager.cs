using System.Collections.Generic;
using TOT.Common;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : SingletonBehaviour<MusicManager>
{
    [SerializeField] List<AudioClip> _music;
    AudioSource _audio;
    float _startVol;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
        _startVol = _audio.volume;
        WorldManager.Instance.OnWorldChange += OnWorldChange;
    }

    private void OnWorldChange(WorldBrain brain)
    {
        if (brain.SceneName != "SCN_World_Hub") PlayRandom();
        else StopMusic();
    }

    public void PlayRandom()
    {
        var music = _music.SelectRandom();
        _audio.DOFade(0, 0.2f).OnComplete(() =>
        {
            _audio.volume = _startVol;
            _audio.Stop();
            _audio.clip = music;
            _audio.Play();
        });
    }

    public void StopMusic()
    {
        _audio.DOFade(0, 0.2f).OnComplete(() => _audio.Stop());
    }
}