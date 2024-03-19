using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraFollow : SingletonBehaviour<CameraFollow>
{
    [SerializeField] Transform _cameraPos;

    void Update()
    {
        transform.position = _cameraPos.transform.position;
    }

    public void Shake(float intensity = 0.5f, float duration = 1f)
    {
        Camera.main.transform.DOShakePosition(duration, intensity, fadeOut: true);
    }
}
