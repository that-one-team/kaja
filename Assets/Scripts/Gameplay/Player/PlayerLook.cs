using System.Collections;
using NaughtyAttributes;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] float _cameraMaxTiltDeg = 45f;
    [SerializeField] float _tiltAngle = 5;
    [SerializeField] float _tiltSpeed = 5;

    [ShowNonSerializedField]
    Vector2 _mouseVec;
    Rigidbody _rb;
    Vector2 _look;
    float _tilt;
    public float AngleOffset { get; set; }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameManager.Instance.IsFrozen || !Player.Instance.IsAlive) return;
        _look = PlayerInputs.Instance.Actions.Look.ReadValue<Vector2>();

        var _kb = PlayerInputs.Instance.Actions.Move.ReadValue<Vector2>();
        _tilt = GetCameraTilt(_kb.x);

        _mouseVec.x += _look.x * GameSettings.Instance.MouseSensitivity;
        _mouseVec.y -= _look.y * GameSettings.Instance.MouseSensitivity;
        _mouseVec.y = Mathf.Clamp(_mouseVec.y, -90, 90);
        Camera.main.transform.localRotation = Quaternion.Euler(_mouseVec.y, _mouseVec.x - AngleOffset, _tilt);

        _rb.MoveRotation(Quaternion.Euler(0, _mouseVec.x - AngleOffset, 0));
    }

    float GetCameraTilt(float input)
    {
        var targ = -input * _tiltAngle;
        targ = Mathf.Clamp(targ, -_cameraMaxTiltDeg, _cameraMaxTiltDeg);
        return Mathf.Lerp(_tilt, targ, _tiltSpeed * Time.deltaTime);
    }
}