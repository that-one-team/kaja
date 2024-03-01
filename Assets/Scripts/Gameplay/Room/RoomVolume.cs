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
    GameObject _blocker;
    [SerializeField] AudioClip _blockerSfx;

    AudioSource _source;

    private void OnValidate()
    {
        if (_room == null) return;
        name = "V_" + _room.FriendlyName + " Volume";

        if (_blockerSfx == null)
            _blockerSfx = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Environment/sfx_door.wav");
    }


    private void Start()
    {
        _room.OnRoomStart += OnRoomStart;
        _room.OnRoomEnd += OnRoomEnd;

        _blocker = transform.GetChild(0).gameObject;
        _blocker.SetActive(false);

        _source = GetComponent<AudioSource>();
        _source.spatialBlend = 0.75f;
    }

    public override void OnEnter()
    {
        if (_room.IsRoomActive) return;
        _room.StartRoom();
    }

    void OnRoomStart(int enemyCount)
    {
        _source.PlayOneShot(_blockerSfx);
        _blocker.SetActive(true);
        _blocker.GetComponent<Collider>().enabled = false;
        _blocker.GetComponentInChildren<ParticleSystem>().Play();
        StartCoroutine(EnableCol());

        IEnumerator EnableCol()
        {
            yield return new WaitForSeconds(0.2f);
            _blocker.GetComponent<Collider>().enabled = true;
        }
    }

    void OnRoomEnd()
    {
        _source.PlayOneShot(_blockerSfx);
        _blocker.transform.DOScaleY(0, 1).OnComplete(() =>
        {
            _blocker.SetActive(false);
        });
        _blocker.GetComponentInChildren<ParticleSystem>().Play();
    }

    public DropdownList<Room> GetRooms() => Room.GetRoomValues();

}