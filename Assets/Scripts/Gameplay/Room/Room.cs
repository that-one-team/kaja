using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TOT.Common;
using UnityEngine;

[Serializable]
public struct RoomSpawnable
{
    public GameObject EnemyToSpawn;
    public float Chance;
}

public class Room : MonoBehaviour
{
    [field: SerializeField] public string FriendlyName { get; private set; }
    public bool IsSubRoom = false;
    public bool IsBossRoom = false;
    [SerializeField] List<RoomSpawnable> _spawnableEnemies = new();

    [field: SerializeField] public int MaxWaves { get; private set; } = 1;
    public int CurrentWave { get; private set; }
    int _enemiesLeft;

    [Tooltip("In seconds")]
    [field: SerializeField] public float WaveCooldown { get; private set; } = 5f;

    [SerializeField] List<EnemySpawner> _spawners = new();

    Transform _roomStart;
    public Transform RoomStartPosition
    {
        get
        {
            if (_roomStart == null)
                _roomStart = _roomStartPositions.SelectRandom().transform.GetChild(0);

            return _roomStart;
        }
    }

    [SerializeField] List<PlayerStart> _roomStartPositions;

    [field: SerializeField] public Transform PortalSpawn { get; private set; }

    public bool IsRoomActive { get; private set; }
    public bool IsRoomComplete { get; private set; }

    public event Action<int> OnRoomStart;
    public event Action OnRoomEnd;

    void Start()
    {
        if (_spawners.Count == 0)
            _spawners = GetComponentsInChildren<EnemySpawner>().ToList();
    }

    public void StartRoom()
    {
        if (IsRoomComplete) return;

        OnRoomStart?.Invoke(0);
        IsRoomActive = true;
        if (!IsSubRoom)
            WorldManager.Instance.CurrentWorld.ChangeRoom(this);

        NextWave();
    }

    public void NextWave()
    {
        CurrentWave++;
        foreach (var spawner in _spawners)
        {
            spawner.Room = this;
            _enemiesLeft += spawner.SpawnEnemies(_spawnableEnemies);
        }
    }

    public void DecreaseEnemy()
    {
        _enemiesLeft--;

        if (_enemiesLeft <= 0)
        {
            StartCoroutine(NewWave());
        }
    }

    IEnumerator NewWave()
    {
        print("Wave has finished!");
        if (CurrentWave >= MaxWaves)
        {
            CompleteRoom();
            yield break;
        }
        yield return new WaitForSeconds(WaveCooldown);
        NextWave();
    }

    public void CompleteRoom()
    {
        print("[ROOM] " + FriendlyName + " complete!");
        OnRoomEnd?.Invoke();
        IsRoomActive = false;
        // WorldManager.Instance.CurrentWorld.ChangeRoom(null);
        IsRoomComplete = true;
        WorldManager.Instance.CurrentWorld.CompleteRoom();
    }

    public static DropdownList<Room> GetRoomValues()
    {
        var rooms = FindObjectsOfType<Room>();
        var list = new DropdownList<Room>();
        foreach (var room in rooms)
        {
            list.Add(room.FriendlyName, room);
        }

        return list;
    }
}
