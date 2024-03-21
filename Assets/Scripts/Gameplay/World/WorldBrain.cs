using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TOT.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldBrain : MonoBehaviour
{
    [Header("General settings")]
    public new string name;
    [Scene] public string SceneName;

    public Scene Scene { get => SceneManager.GetSceneByName(SceneName); }

    [field: SerializeField] public GameObject RoomPortal { get; private set; }
    [SerializeField] Camera _roomPortalCamera;
    [SerializeField] GameObject _worldPortal;
    [field: SerializeField] public PlayerStart PlayerSpawnPoint { get; private set; }
    public bool HasCustomRooms = true;
    public bool IsHub = false;

    [Header("Room spawner")]
    [SerializeField] int _maxRooms;
    [SerializeField] float _roomSpacing = 50;
    [SerializeField] List<GameObject> _roomsPrefabs;
    [SerializeField] GameObject _bossRoomPrefab;
    [SerializeField] Queue<Room> _roomPool = new();

    [field: ShowNonSerializedField]
    public Room NextRoom { get; private set; }
    [field: ShowNonSerializedField]
    public Room CurrentRoom { get; private set; }

    public event Action<Room> OnChangeRoom;
    public event Action OnRoomComplete;

    private void Start()
    {
        if (_roomsPrefabs.Count > 0 && !HasCustomRooms)
            SpawnRooms();

        if (WorldManager.Instance.CurrentWorld == null)
            WorldManager.Instance.ChangeWorld(this, () =>
            {
                PlayerSpawnPoint.TeleportPlayer();
            });

        if (IsHub || HasCustomRooms) return;
        _worldPortal.SetActive(false);
    }

    public void SpawnRooms()
    {
        var normalRooms = _roomsPrefabs.Where(r => !r.GetComponent<Room>().IsBossRoom).ToList();
        var shuffled = _roomsPrefabs.Shuffle();

        for (int i = 0; i < _maxRooms; i++)
        {
            var room = shuffled[i >= shuffled.Count ? 0 : i];

            _roomPool.Enqueue(Instantiate(room, _roomSpacing * i * transform.right, Quaternion.identity, transform).GetComponent<Room>());
        }

        if (_bossRoomPrefab)
            _roomPool.Enqueue(Instantiate(_bossRoomPrefab, _roomSpacing * _maxRooms * transform.right, Quaternion.identity).GetComponent<Room>());

        // get player start in first room
        if (PlayerSpawnPoint == null)
            PlayerSpawnPoint = _roomPool.Peek().RoomStartPosition.GetComponentInParent<PlayerStart>();

        ChangeRoom(null);
        MoveCameraToNextRoom();
    }

    public void ChangeRoom(Room room)
    {
        CurrentRoom = room;
        OnChangeRoom?.Invoke(room);

        NextRoom = _roomPool.Count > 0 ? _roomPool.Dequeue() : null;

        if (IsHub || HasCustomRooms) return;
        RoomPortal.SetActive(NextRoom != null);
        if (!_roomPool.TryPeek(out _) && NextRoom != null)
        {
            print("out of rooms! setting current room as world portal room");
            NextRoom.SetWorldPortal(_worldPortal);
        }
    }

    public void CompleteRoom()
    {
        OnRoomComplete?.Invoke();
        if (HasCustomRooms) return;
        MoveCameraToNextRoom();
    }

    public void MoveCameraToNextRoom()
    {
        if (_roomPortalCamera == null) return;
        var camPos = NextRoom.RoomStartPosition;
        _roomPortalCamera.transform.SetPositionAndRotation(camPos.position, camPos.rotation);

        _roomPortalCamera.GetComponent<RoomCamera>().SetPositionInRoom(NextRoom);
    }

    public void HidePreviousRoom()
    {
        CurrentRoom.gameObject.SetActive(false);
    }
}

