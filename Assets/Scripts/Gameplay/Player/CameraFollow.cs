using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform _cameraPos;
    [SerializeField] float _eyeHeight = 1.52f; // 1.52m idk why

    void Update()
    {
        transform.position = _cameraPos.transform.position;
    }
}
