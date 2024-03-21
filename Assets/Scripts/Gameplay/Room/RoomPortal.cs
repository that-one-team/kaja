using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    public Transform OffsetPos;
    public DoorInteractable Door;
    WorldBrain _currentWorld;

    private void Start()
    {
        WorldManager.Instance.OnWorldChange += Init;
        // Door.IsLocked = true;
    }

    private void Init(WorldBrain brain)
    {
        if (_currentWorld != null)
        {
            _currentWorld.OnChangeRoom -= MoveToRoom;
            _currentWorld.OnRoomComplete -= CompletedRoom;
        }

        _currentWorld = brain;
        _currentWorld.OnChangeRoom += MoveToRoom;
        _currentWorld.OnRoomComplete += CompletedRoom;
    }

    private void OnDisable()
    {
        WorldManager.Instance.OnWorldChange -= Init;
        _currentWorld = null;
    }

    private void CompletedRoom()
    {
        Door.IsLocked = false;
        WorldManager.Instance.CurrentWorld.MoveCameraToNextRoom();
    }

    public void MoveToRoom(Room room)
    {
        if (room == null || room.PortalSpawn == null) return;
        transform.SetPositionAndRotation(room.PortalSpawn.position, room.PortalSpawn.rotation);
        Misc.SnapToGround(gameObject);

        Door.ForceInteract(false);
        // Door.IsLocked = true;
    }
}
