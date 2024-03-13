using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ContractItem : MonoBehaviour
{
    [SerializeField] AudioClip _healAudio;

    AudioSource _audio;
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        Player.Instance.OnHealthChanged += OnHurt;
    }

    private void OnHurt(int changed, int health)
    {
        if (health > 0) return;

        Player.Instance.Heal(Player.Instance.MaxHealth + Mathf.Abs(changed));
        _audio.PlayOneShot(_healAudio, 0.6f);
    }
}
