using System;
using UnityEngine;

public class RoomPortal : SingletonBehaviour<RoomPortal>
{
    public Transform OffsetPos;
    [SerializeField] DoorInteractable _door;

    private void Start()
    {
        WorldBrain.Instance.OnChangeRoom += MoveToRoom;
        WorldBrain.Instance.OnRoomComplete += CompletedRoom;
    }

    private void CompletedRoom()
    {
        _door.IsLocked = false;
        WorldBrain.Instance.MoveCameraToNextRoom();
    }

    public void MoveToRoom(Room room)
    {
        if (room == null) return;
        _door.Interact();
        _door.IsLocked = true;
        transform.SetPositionAndRotation(room.PortalSpawn.position, room.PortalSpawn.rotation);
        Misc.SnapToGround(gameObject);
    }
}
