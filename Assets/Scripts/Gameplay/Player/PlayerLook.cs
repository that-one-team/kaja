using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Range(0, 1)] public float MouseSensitivity = 0.5f;
    [SerializeField] float _cameraMaxTiltDeg = 45f;
    [SerializeField] float _tiltAngle = 5;
    [SerializeField] float _tiltSpeed = 5;
    Vector2 _mouseVec;
    Rigidbody _rb;
    Vector2 _input;
    float _tilt;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameManager.Instance.IsFrozen) return;

        _input = PlayerInputs.Instance.Actions.Move.ReadValue<Vector2>();
        _tilt = GetCameraTilt(_input.x);

        _mouseVec.x += PlayerInputs.Instance.Actions.Look.ReadValue<Vector2>().x * MouseSensitivity;
        _mouseVec.y -= PlayerInputs.Instance.Actions.Look.ReadValue<Vector2>().y * MouseSensitivity;
        _mouseVec.y = Mathf.Clamp(_mouseVec.y, -90, 90);
        Camera.main.transform.localRotation = Quaternion.Euler(_mouseVec.y, _mouseVec.x, _tilt);

        _rb.MoveRotation(Quaternion.Euler(0, _mouseVec.x, 0));
    }

    float GetCameraTilt(float input)
    {
        var targ = -input * _tiltAngle;
        targ = Mathf.Clamp(targ, -_cameraMaxTiltDeg, _cameraMaxTiltDeg);
        return Mathf.Lerp(_tilt, targ, _tiltSpeed * Time.deltaTime);
    }
}