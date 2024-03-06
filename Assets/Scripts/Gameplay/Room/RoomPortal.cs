using System;
using UnityEngine;

public class RoomPortal : SingletonBehaviour<RoomPortal>
{
    public Transform OffsetPos;
    public DoorInteractable Door;

    private void Start()
    {
        WorldManager.Instance.CurrentWorld.OnChangeRoom += MoveToRoom;
        WorldManager.Instance.CurrentWorld.OnRoomComplete += CompletedRoom;
    }

    private void CompletedRoom()
    {
        Door.IsLocked = false;
        WorldManager.Instance.CurrentWorld.MoveCameraToNextRoom();
    }

    public void MoveToRoom(Room room)
    {
        if (room == null) return;
        Door.Interact();
        Door.IsLocked = true;
        transform.SetPositionAndRotation(room.PortalSpawn.position, room.PortalSpawn.rotation);
        Misc.SnapToGround(gameObject);
    }
}
