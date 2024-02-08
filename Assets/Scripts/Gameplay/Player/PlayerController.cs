using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _moveSpeed = 10.0f;
    [SerializeField] float _jumpForce = 20;
    [SerializeField] float _groundDrag;
    [SerializeField] float _airDrag;
    [SerializeField] float _airMultiplier;

    [Header("Ground Check")]
    [SerializeField] float _groundCheckOffset = 0.1f;
    [SerializeField] Vector3 _groundCheckSize = Vector3.one / 2;
    [SerializeField] LayerMask _groundMask;

    [Header("Slope")]
    [SerializeField] float _slopeSpeedMultiplier = 20f;
    [SerializeField] float _slopeStickForce = 80f;
    [SerializeField] float _maxSlopeAngle;
    RaycastHit _slope;

    Vector2 _inputVec;
    bool _isGrounded = false;

    Rigidbody _rb;
    Vector3 _vel;

    void Start()
    {
        PlayerInputs.Instance.Actions.Jump.performed += OnJump;

        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (!_isGrounded) return;
        _rb.velocity = new(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    void Update()
    {
        _isGrounded = Physics.CheckBox(transform.position + Vector3.up * _groundCheckOffset, _groundCheckSize, Quaternion.identity, _groundMask);
        _inputVec = PlayerInputs.Instance.Actions.Move.ReadValue<Vector2>();

        ControlSpeed();
    }

    private void FixedUpdate()
    {
        _vel = (transform.right * _inputVec.x + transform.forward * _inputVec.y).normalized * _moveSpeed;

        if (IsOnSlope())
        {
            _rb.AddForce(_moveSpeed * _slopeSpeedMultiplier * GetSlopeDir(), ForceMode.Force);

            if (_rb.velocity.y > 0)
                _rb.AddForce(Vector3.down * _slopeStickForce, ForceMode.Force);
        }

        var mult = _isGrounded ? 1 : _airMultiplier;
        _rb.AddForce(10f * mult * _vel, ForceMode.Force);
    }

    void ControlSpeed()
    {
        _rb.drag = _isGrounded ? _groundDrag : 0;

        if (IsOnSlope() && _rb.velocity.magnitude > _moveSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * _moveSpeed;
            return;
        }

        Vector3 rawVel = new(_rb.velocity.x, 0, _rb.velocity.z);

        if (rawVel.magnitude > _moveSpeed)
        {
            var clampedVel = rawVel.normalized * _moveSpeed;
            _rb.velocity = new(clampedVel.x, _rb.velocity.y, clampedVel.z);
        }
    }

    bool IsOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slope, 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slope.normal);
            return angle < _maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeDir() => Vector3.ProjectOnPlane(_vel, _slope.normal).normalized;

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 1000, 100), $"Vel: {_vel.x} | {_vel.y} | {_vel.z}\nSpeed: {_rb.velocity.magnitude}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isGrounded ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * _groundCheckOffset, _groundCheckSize / 2);
    }
#endif
}
