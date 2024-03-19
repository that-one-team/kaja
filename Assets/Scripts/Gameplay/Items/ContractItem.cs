using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ContractItem : MonoBehaviour
{
    bool _isUsed;
    [SerializeField] AudioClip _healAudio;
    AudioSource _audio;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
        Player.Instance.OnHealthChanged += OnHurt;
    }

    private void OnDisable()
    {
        Player.Instance.OnHealthChanged -= OnHurt;
    }

    private void OnHurt(int changed, int health)
    {
        if (health > 0 || _isUsed) return;

        Notifications.Instance.Notify("<b><color=\"yellow\">You have been given another chance...</color></b>");
        Player.Instance.Heal(Player.Instance.MaxHealth + Mathf.Abs(changed));
        _audio.PlayOneShot(_healAudio, 0.6f);
        _isUsed = true;

        PlayerInventory.Instance.RemoveItem("Mangagaway's Contract");
        Destroy(gameObject, 3);
    }
}
