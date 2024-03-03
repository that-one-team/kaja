using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    Transform _playerCam;
    Transform _roomPortal;
    Transform _prevPortal;

    void Start()
    {
        _playerCam = Camera.main.transform;
    }
}
