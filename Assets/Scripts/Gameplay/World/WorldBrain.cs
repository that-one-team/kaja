using System;
using System.Collections.Generic;
using System.Linq;
using TOT.Common;
using UnityEngine;

public class WorldBrain : SingletonBehaviour<WorldBrain>
{
    [Header("General settings")]
    public new string name;
    [SerializeField] Camera _roomPortalCamera;

    [Header("Room spawner")]
    [SerializeField] int _maxRooms;
    [SerializeField] float _roomSpacing = 50;
    [SerializeField] List<GameObject> _roomsPrefabs;
    [SerializeField] List<Room> _roomPool = new();

    public Room NextRoom { get; private set; }
    public Room CurrentRoom { get; private set; }

    int _lastRoomIdx = -1;

    public event Action<Room> OnChangeRoom;

    private void Start()
    {
        if (_roomsPrefabs.Count == 0) return;
        SpawnRooms();
    }

    public void SpawnRooms()
    {
        var normalRooms = _roomsPrefabs.Where(r => !r.GetComponent<Room>().IsBossRoom).ToList();
        var shuffled = _roomsPrefabs.Shuffle();

        for (int i = 0; i < _maxRooms; i++)
        {
            var room = shuffled[i >= shuffled.Count ? 0 : i];

            _roomPool.Add(Instantiate(room, _roomSpacing * i * Vector3.right, Quaternion.identity).GetComponent<Room>());
        }

        // set room camera to next room
        ChangeRoom(null);
        MoveCameraToNextRoom();
    }

    public void ChangeRoom(Room room)
    {
        if (room != null)
            _lastRoomIdx = _roomPool.LastIndexOf(CurrentRoom);
        CurrentRoom = room;
        OnChangeRoom?.Invoke(room);
        NextRoom = _roomPool[_lastRoomIdx + 1];
    }

    public void MoveCameraToNextRoom()
    {
        if (_roomPortalCamera == null) return;
        var camPos = NextRoom.RoomStartPosition;
        _roomPortalCamera.transform.SetPositionAndRotation(camPos.position, camPos.rotation);

        _roomPortalCamera.GetComponent<RoomCamera>().SetPositionInRoom(NextRoom.RoomStartPosition);
    }
}
