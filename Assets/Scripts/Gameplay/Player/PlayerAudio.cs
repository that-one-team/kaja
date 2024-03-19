using System.Collections.Generic;
using TOT.Common;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : SingletonBehaviour<PlayerAudio>
{
    [Header("Gameplay")]
    [SerializeField] List<AudioClip> _hurtSfx;
    [SerializeField] AudioClip _deathSfx;

    [Header("World")]
    [SerializeField] AudioClip _roomFinishedSfx;

    [Header("Footsteps")]
    [SerializeField] AudioClip[] _footstepSounds;
    [SerializeField] Vector2 _timeBetweenSteps = new(0.3f, 0.6f);

    AudioSource _audio;
    float _timeSinceLastStep;

    bool _isWalking = false;
    PlayerController _player;
    Rigidbody _rb;

    void Start()
    {
        _player = GetComponent<PlayerController>();
        _rb = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioSource>();
        Player.Instance.OnHealthChanged += OnHurt;
        Player.Instance.OnDie += OnDie;

        WorldManager.Instance.OnWorldChange += OnWorldChange;
    }

    private void OnDie(LivingBeing being)
    {
        _audio.PlayOneShot(_deathSfx);
    }

    private void OnHurt(int changed, int current)
    {
        if (changed > 0) return;
        _audio.PlayOneShot(_hurtSfx.SelectRandom(), 3);
    }

    private void OnDisable()
    {
        WorldManager.Instance.OnWorldChange -= OnWorldChange;
        Player.Instance.OnHealthChanged -= OnHurt;
        Player.Instance.OnDie -= OnDie;
    }

    private void OnWorldChange(WorldBrain brain)
    {
        brain.OnRoomComplete += () => _audio.PlayOneShot(_roomFinishedSfx, 2);
    }

    void Update()
    {
        _isWalking = new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude > 10f && _player.IsGrounded && !_player.IsSliding;
        if (!_isWalking) return;

        if (Time.time - _timeSinceLastStep >= Random.Range(_timeBetweenSteps.x, _timeBetweenSteps.y))
        {
            var clip = _footstepSounds.SelectRandom();
            _audio.PlayOneShot(clip);

            _timeSinceLastStep = Time.time;
        }
    }
}
