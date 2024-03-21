using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    Transform _player;

    WorldBrain _world;

    private void Start()
    {
        WorldManager.Instance.OnWorldChange += OnWorldChange;
    }

    private void OnWorldChange(WorldBrain brain)
    {
        _player = PlayerController.Instance.transform;
        _world = WorldManager.Instance.CurrentWorld;
    }

    private void OnDisable()
    {
        WorldManager.Instance.OnWorldChange -= OnWorldChange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponentInParent<RoomPortal>().Door.IsLocked) return;
        if (other.CompareTag("Player"))
        {
            var portalOffset = _player.position - transform.position;

            var rot = -Quaternion.Angle(transform.rotation, _world.NextRoom.RoomStartPosition.rotation);
            rot += 180;

            _player.Rotate(Vector3.up, rot);

            var pos = Quaternion.Euler(0, rot, 0) * portalOffset;
            _player.position = _world.NextRoom.RoomStartPosition.position + pos;

            WorldManager.Instance.CurrentWorld.HidePreviousRoom();
        }
    }


}
