using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RoomVolume : Volume
{
    [Dropdown("GetRooms")]
    [SerializeField] Room _room;
    [SerializeField] List<Transform> _blockerSpawnZones = new();
    public GameObject BlockerPrefab;
    [SerializeField] AudioClip _blockerSfx;
    [SerializeField] bool _spawnBlockers = true;

    readonly List<GameObject> _blockers = new();

    AudioSource _source;

    private void OnValidate()
    {
        if (_room == null) return;
        name = "V_" + _room.FriendlyName + " Volume";

        _blockerSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Environment/sfx_treedoor.wav");
    }


    private void Start()
    {
        _room.OnRoomStart += OnRoomStart;
        _room.OnRoomEnd += OnRoomEnd;

        _source = GetComponent<AudioSource>();
        _source.spatialBlend = 0.75f;

        if (_blockerSpawnZones.Count == 0) return;
        foreach (var spawn in _blockerSpawnZones)
        {
            var blocker = Instantiate(BlockerPrefab, spawn.transform.position, spawn.transform.rotation, spawn);
            blocker.SetActive(false);
            _blockers.Add(blocker);
        }
    }

    public override void OnEnter()
    {
        if (_room.IsRoomActive) return;
        _room.StartRoom();
    }

    void OnRoomStart(int enemyCount)
    {
        if (!_spawnBlockers) return;
        _source.PlayOneShot(_blockerSfx);
        foreach (var blocker in _blockers)
        {
            blocker.SetActive(true);
            blocker.GetComponent<Collider>().enabled = false;
            blocker.GetComponentInChildren<ParticleSystem>().Play();
            StartCoroutine(EnableCol(blocker));
        }

        static IEnumerator EnableCol(GameObject blocker)
        {
            yield return new WaitForSeconds(0.2f);
            blocker.GetComponent<Collider>().enabled = true;
        }
    }

    void OnRoomEnd()
    {
        if (!_spawnBlockers) return;
        _source.PlayOneShot(_blockerSfx);
        foreach (var blocker in _blockers)
        {
            blocker.transform.DOScaleY(0, 1).OnComplete(() =>
            {
                blocker.SetActive(false);
            });
            blocker.GetComponentInChildren<ParticleSystem>().Play();
        }
    }

    public DropdownList<Room> GetRooms() => Room.GetRoomValues();

}