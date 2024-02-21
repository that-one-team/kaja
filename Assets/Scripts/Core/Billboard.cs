using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] bool _lockY = true;
    [SerializeField] bool _flip = true;

    void Start()
    {
        var flippedX = _flip ? -transform.localScale.x : transform.localScale.x;
        transform.localScale = new Vector3(flippedX, transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
        if (_lockY) transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}
