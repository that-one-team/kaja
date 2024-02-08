using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Range(0, 1)] public float MouseSensitivity = 0.5f;
    Vector2 _mouseVec;
    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _mouseVec.x += PlayerInputs.Instance.Actions.Look.ReadValue<Vector2>().x * MouseSensitivity;
        _mouseVec.y -= PlayerInputs.Instance.Actions.Look.ReadValue<Vector2>().y * MouseSensitivity;
        _mouseVec.y = Mathf.Clamp(_mouseVec.y, -90, 90);
        Camera.main.transform.localRotation = Quaternion.Euler(_mouseVec.y, 0, 0);

        _rb.MoveRotation(Quaternion.Euler(0, _mouseVec.x, 0));
    }
}