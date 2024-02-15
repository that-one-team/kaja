using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RoomVolume : Volume
{
    [Dropdown("GetRooms")]
    [SerializeField] Room _room;
    GameObject _blocker;
    [SerializeField] AudioClip _blockerSfx;

    AudioSource _source;

    private void OnValidate()
    {
        if (_room == null) return;
        name = "V_" + _room.FriendlyName + " Volume";

        _source = GetComponent<AudioSource>();
        _source.spatialBlend = 0.75f;
    }


    private void Start()
    {
        _room.OnRoomStart += OnRoomStart;
        _room.OnRoomEnd += OnRoomEnd;

        _blocker = transform.GetChild(0).gameObject;
        _blocker.SetActive(false);

        _source = GetComponent<AudioSource>();
    }

    public override void OnEnter()
    {
        if (_room.IsRoomActive) return;
        _room.StartRoom();
    }

    void OnRoomStart(int enemyCount)
    {
        _source.PlayOneShot(_blockerSfx);
        _blocker.transform.localScale = new Vector3(_blocker.transform.localScale.x, 0, _blocker.transform.localScale.z);
        _blocker.SetActive(true);
        _blocker.transform.DOScaleY(1, 1);
    }

    void OnRoomEnd()
    {
        _source.PlayOneShot(_blockerSfx);
        _blocker.transform.DOScaleY(0, 1).OnComplete(() =>
        {
            _blocker.SetActive(false);
        });
    }

    public DropdownList<Room> GetRooms() => Room.GetRoomValues();

}