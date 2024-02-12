using System.Collections;
using System.Collections.Generic;
using TOT.Common;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] AudioClip[] _footstepSounds;
    [SerializeField] Vector2 _timeBetweenSteps = new(0.3f, 0.6f);

    AudioSource _source;
    float _timeSinceLastStep;

    bool _isWalking = false;
    PlayerController _player;
    Rigidbody _rb;

    void Start()
    {
        _player = GetComponent<PlayerController>();
        _rb = GetComponent<Rigidbody>();
        _source = GetComponent<AudioSource>();
    }

    void Update()
    {
        _isWalking = new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude > 10f && _player.IsGrounded && !_player.IsSliding;
        if (!_isWalking) return;

        if (Time.time - _timeSinceLastStep >= Random.Range(_timeBetweenSteps.x, _timeBetweenSteps.y))
        {
            var clip = _footstepSounds.SelectRandom();
            _source.PlayOneShot(clip);

            _timeSinceLastStep = Time.time;
        }
    }
}
