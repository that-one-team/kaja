using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldPortal : MonoBehaviour
{
    Transform _player;
    bool _hasLoaded;
    public DoorInteractable Door;

    void Start()
    {
        WorldManager.Instance.OnWorldChange += OnWorldChange;
    }

    private void OnWorldChange(WorldBrain brain)
    {
        _player = PlayerController.Instance.transform;
    }

    private void OnDisable()
    {
        WorldManager.Instance.OnWorldChange -= OnWorldChange;
    }

    void Update()
    {
        if (_player == null)
        {
            _player = PlayerController.Instance.transform;
            return;
        }
        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist < 4 && !_hasLoaded)
        {
            WorldManager.Instance.LoadNextWorld();
            _hasLoaded = true;
            Door.IsLocked = false;
        }
    }
}
