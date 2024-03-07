using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldPortal : MonoBehaviour
{
    Transform _player;
    bool _hasLoaded;
    DoorInteractable _door;

    void Start()
    {
        WorldManager.Instance.OnWorldChange += (WorldBrain brain) =>
        {
            _player = PlayerController.Instance.transform;
        };
        _door = GetComponentInChildren<DoorInteractable>();
        _door.IsLocked = true;
    }

    void Update()
    {
        if (_player == null) return;
        var dist = Vector3.Distance(transform.position, _player.position);
        if (dist < 4 && !_hasLoaded)
        {
            WorldManager.Instance.LoadNextWorld();
            _hasLoaded = true;
            _door.IsLocked = false;
        }
    }
}