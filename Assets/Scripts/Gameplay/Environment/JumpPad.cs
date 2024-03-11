using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class JumpPad : MonoBehaviour
{
    [SerializeField] float _bounceForce = 20;
    [SerializeField] int _segments = 20;
    [SerializeField] float _segmentDuration = 0.05f;

    [SerializeField][Layer] int _selfLayer;

    bool _isPlayerGrounded;
    LineRenderer _line;

    List<Vector3> _linePos = new();

    void Update()
    {
        _isPlayerGrounded = PlayerController.Instance.IsGrounded;
    }

    private void OnValidate()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && _isPlayerGrounded)
            Bounce();
    }

    void Bounce()
    {
        var rb = PlayerController.Instance.GetComponent<Rigidbody>();
        rb.AddForce(transform.up * _bounceForce, ForceMode.Impulse);
    }
}
