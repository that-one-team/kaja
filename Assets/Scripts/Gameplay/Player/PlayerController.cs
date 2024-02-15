using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : SingletonBehaviour<PlayerController>
{
    [Header("Movement")]
    [SerializeField] float _moveSpeed = 10.0f;
    [SerializeField] float _jumpForce = 20;
    [SerializeField] float _groundDrag;
    [SerializeField] float _airDrag;
    [SerializeField] float _airMultiplier;
    float _currMoveSpeed;

    [Header("Ground Check")]
    [SerializeField] float _groundCheckOffset = 0.1f;
    [SerializeField] Vector3 _groundCheckSize = Vector3.one / 2;
    [SerializeField] LayerMask _groundMask;

    [Header("Sliding")]
    [SerializeField] float _slideHeight = 0.5f;
    [SerializeField] float _slideSpeed = 12f;
    float _startHeight;
    public bool IsSliding { get; private set; }
    Vector3 _slideDir;

    [Header("Slope")]
    [SerializeField] float _slopeSpeedMultiplier = 20f;
    [SerializeField] float _slopeStickForce = 80f;
    [SerializeField] float _maxSlopeAngle;
    RaycastHit _slope;
    Vector2 _inputVec;
    public bool IsGrounded { get; private set; }
    bool _isJumping = false;
    bool _hasSlideDir = false;

    Rigidbody _rb;
    Vector3 _vel;

    void Start()
    {
        PlayerInputs.Instance.Actions.Jump.performed += OnJump;

        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _currMoveSpeed = _moveSpeed;

        _startHeight = transform.localScale.y;
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (!IsGrounded || GameManager.Instance.IsFrozen) return;
        _isJumping = true;
        _rb.velocity = new(_rb.velocity.x, 0, _rb.velocity.z);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);

        Invoke(nameof(ResetJump), 0.2f);
    }

    void ResetJump()
    {
        _isJumping = false;
    }

    void CheckSlide()
    {
        if (!IsGrounded || GameManager.Instance.IsFrozen) return;

        // TODO find a way to use new input system (new input system does not have hold button)
        IsSliding = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftShift);
        if (!IsSliding)
        {
            _slideDir = transform.forward;
            _hasSlideDir = false;
        }
    }


    void Update()
    {
        IsGrounded = Physics.CheckBox(transform.position + Vector3.up * _groundCheckOffset, _groundCheckSize, Quaternion.identity, _groundMask);

        if (GameManager.Instance.IsFrozen)
        {
            _rb.velocity = Vector3.up * _rb.velocity.y;
            return;
        }

        _inputVec = PlayerInputs.Instance.Actions.Move.ReadValue<Vector2>();

        _rb.useGravity = !IsOnSlope();
        ControlSpeed();

        CheckSlide();
        SetHeight();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsFrozen) return;
        _vel = (transform.right * _inputVec.x + transform.forward * _inputVec.y).normalized * _currMoveSpeed;

        if (IsOnSlope() && !_isJumping)
        {
            _rb.AddForce(_currMoveSpeed * _slopeSpeedMultiplier * GetSlopeDir(), ForceMode.Force);

            if (_rb.velocity.y > 0)
                _rb.AddForce(Vector3.down * _slopeStickForce, ForceMode.Force);
        }

        if (IsSliding)
        {
            DoSlide();
        }
        else
        {
            var mult = IsGrounded ? 1 : _airMultiplier;
            _rb.AddForce(10f * mult * _vel, ForceMode.Force);
        }
    }

    void DoSlide()
    {
        if (_vel.normalized.magnitude > 0 && !_hasSlideDir)
        {
            _slideDir = _vel.normalized;
            _hasSlideDir = true;
        }
        _rb.AddForce(_slideSpeed * 10f * _slideDir, ForceMode.Force);
    }

    void SetHeight()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, IsSliding ? _slideHeight : _startHeight, transform.localScale.z), 20 * Time.deltaTime);
    }

    void ControlSpeed()
    {
        _rb.drag = IsGrounded ? _groundDrag : 0;

        if (IsOnSlope() && _rb.velocity.magnitude > _currMoveSpeed && !_isJumping)
        {
            _rb.velocity = _rb.velocity.normalized * _currMoveSpeed;
            return;
        }

        Vector3 rawVel = new(_rb.velocity.x, 0, _rb.velocity.z);

        if (rawVel.magnitude > _currMoveSpeed)
        {
            var clampedVel = rawVel.normalized * _currMoveSpeed;
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
        Gizmos.color = IsGrounded ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position + Vector3.up * _groundCheckOffset, _groundCheckSize / 2);
    }
#endif
}
