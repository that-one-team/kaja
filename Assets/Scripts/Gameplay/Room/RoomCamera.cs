using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    Transform _playerCam;
    Transform _roomPortal;
    Transform _prevPortal;

    [SerializeField] Material _renderTextureMaterial;

    void Start()
    {
        _playerCam = Camera.main.transform;
        Camera cam = GetComponent<Camera>();

        if (_renderTextureMaterial != null) cam.targetTexture.Release();

        cam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _renderTextureMaterial.mainTexture = cam.targetTexture;
    }

    public void SetPositionInRoom(Room room)
    {
        _roomPortal = room.RoomStartPosition;
        _prevPortal = RoomPortal.Instance.OffsetPos;
    }

    private void Update()
    {
        if (_roomPortal == null || _prevPortal == null) return;
        var offset = _playerCam.position - _prevPortal.position;
        transform.position = _roomPortal.position + offset;

        float angle = Quaternion.Angle(_roomPortal.rotation, _prevPortal.rotation);
        Quaternion rotDiff = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 newCamDir = rotDiff * _playerCam.forward;
        transform.rotation = Quaternion.LookRotation(newCamDir, Vector3.up);
    }
}