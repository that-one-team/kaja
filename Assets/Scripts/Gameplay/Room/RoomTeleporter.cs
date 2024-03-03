using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    bool _isPlayerEnter;
    Transform _player;

    WorldBrain _world;

    private void Start()
    {
        _player = PlayerController.Instance.transform;
        _world = WorldBrain.Instance;
    }

    private void Update()
    {
        if (!_isPlayerEnter) return;
        var portalOffset = _player.position - transform.position;
        float dot = Vector3.Dot(transform.up, portalOffset);

        if (dot < 0)
        {
            var rot = -Quaternion.Angle(transform.rotation, _world.NextRoom.RoomStartPosition.rotation);
            rot += 180;

            _player.Rotate(Vector3.up, rot);

            var pos = Quaternion.Euler(0, rot, 0) * portalOffset;
            _player.position = _world.NextRoom.RoomStartPosition.position + pos;

            _isPlayerEnter = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _isPlayerEnter = true;
    }


}
