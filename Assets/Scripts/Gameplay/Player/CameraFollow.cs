using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform _cameraPos;

    void Update()
    {
        transform.position = _cameraPos.transform.position;
    }
}
