using System.Collections.Generic;
using TOT.Common;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : SingletonBehaviour<MusicManager>
{
    [SerializeField] List<AudioClip> _music;
    AudioSource _audio;
    float _startVol;

    Queue<AudioClip> _musicQueue = new();

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
        _startVol = _audio.volume;
        WorldManager.Instance.OnWorldChange += OnWorldChange;

        ReloadMusic();
    }

    void ReloadMusic()
    {
        var shuffled = _music.Shuffle();
        foreach (var music in shuffled)
        {
            _musicQueue.Enqueue(music);
        }
        _audio.clip = _musicQueue.Dequeue();
    }

    private void OnWorldChange(WorldBrain brain)
    {
        if (brain.SceneName != "SCN_World_Hub") PlayMusic();
        else StopMusic();
    }

    public void PlayMusic()
    {
        _audio.volume = _startVol;
        _audio.Play();
    }

    public void StopMusic()
    {
        _audio.DOFade(0, 0.2f).OnComplete(() =>
            _audio.Stop()
        );
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L)) StopMusic();
#endif
        if (_audio.isPlaying) return;
        if (_musicQueue.TryDequeue(out AudioClip clip))
            _audio.clip = clip;
        else ReloadMusic();

        _audio.Play();
        _audio.volume = _startVol;
    }
}