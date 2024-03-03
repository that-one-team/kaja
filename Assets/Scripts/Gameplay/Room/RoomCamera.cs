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

    public void SetPositionInRoom(Transform roomStart)
    {
        _roomPortal = roomStart;
        _prevPortal = RoomPortal.Instance.OffsetPos;
    }

    private void Update()
    {
        if (_roomPortal == null || _prevPortal == null) return;
        var offset = _playerCam.position - _prevPortal.position;
        transform.position = _roomPortal.position + offset;
    }
}
