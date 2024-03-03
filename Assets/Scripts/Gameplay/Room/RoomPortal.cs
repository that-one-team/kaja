using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : SingletonBehaviour<RoomPortal>
{
    public Transform OffsetPos;

    public void MoveToRoom(Room room)
    {
        transform.SetPositionAndRotation(room.PortalSpawn.position, room.PortalSpawn.rotation);
    }
}
