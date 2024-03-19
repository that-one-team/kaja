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
        _audio.Play();
        StartCoroutine(WaitTillEndOfMusic());
    }

    IEnumerator WaitTillEndOfMusic()
    {
        yield return new WaitUntil(() => !_audio.isPlaying);
        yield return new WaitForSeconds(1);
        if (_musicQueue.TryDequeue(out AudioClip clip))
            _audio.clip = clip;
        else ReloadMusic();
    }

    public void StopMusic()
    {
        _audio.DOFade(0, 0.2f).OnComplete(() => _audio.Stop());
    }
}