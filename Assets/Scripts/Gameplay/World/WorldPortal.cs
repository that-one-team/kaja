using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldPortal : MonoBehaviour
{
    Transform _player;
    bool _hasLoaded;
    [field: SerializeField] public DoorInteractable Door { get; private set; }

    void Start()
    {
        WorldManager.Instance.OnWorldChange += (WorldBrain brain) =>
        {
            _player = PlayerController.Instance.transform;
        };
        Door.IsLocked = true;

        // i hate this bruteforcing but i dont have time
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
