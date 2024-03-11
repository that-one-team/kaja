using System;
using UnityEngine;

public class RoomPortal : SingletonBehaviour<RoomPortal>
{
    public Transform OffsetPos;
    public DoorInteractable Door;

    private void Start()
    {
        WorldManager.Instance.OnWorldChange += Init;
        Door.IsLocked = true;
    }

    private void Init(WorldBrain brain)
    {
        brain.OnChangeRoom += MoveToRoom;
        brain.OnRoomComplete += CompletedRoom;
    }

    private void CompletedRoom()
    {
        Door.IsLocked = false;
        WorldManager.Instance.CurrentWorld.MoveCameraToNextRoom();
    }

    public void MoveToRoom(Room room)
    {
        if (room == null || room.PortalSpawn == null) return;
        Door.Interact();
        Door.IsLocked = true;
        transform.SetPositionAndRotation(room.PortalSpawn.position, room.PortalSpawn.rotation);
        Misc.SnapToGround(gameObject);
    }
}
