using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [SerializeField] float _bobHeight = 0.5f;

    Vector3 _startPos;
    Vector3 _bob;

    void Start()
    {
        _startPos = transform.localPosition;
    }

    void Update()
    {
        float _bobSpeed = Mathf.Abs(PlayerController.Instance.GetComponent<Rigidbody>().velocity.magnitude);

        float yOff = Mathf.Sin(Time.time * _bobSpeed) * _bobHeight;
        float xOff = Mathf.Sin(Time.time * _bobSpeed) * _bobHeight;
        _bob = new Vector3(_startPos.x + xOff, _startPos.y + yOff, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, _bob, 10 * Time.deltaTime);
    }
}
