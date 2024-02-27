using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [SerializeField] float _bobHeight = 0.5f;
    [SerializeField] float _speed = 10;

    float _bobSpeed;
    Vector3 _startPos;
    Vector3 _bob;

    void Start()
    {
        _startPos = transform.localPosition;
    }

    void Update()
    {
        _bobSpeed = Mathf.Abs(PlayerInputs.Instance.Actions.Move.ReadValue<Vector2>().magnitude) * _speed;

        float xOff = Mathf.Cos(Time.time * _bobSpeed / 2) * _bobHeight * 2;
        float yOff = Mathf.Sin(Time.time * _bobSpeed) * _bobHeight;
        _bob = new Vector3(_startPos.x + xOff, _startPos.y + yOff, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, _bob, 10 * Time.deltaTime);
    }

}
