using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] bool _lockY = true;
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        if (_lockY) transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}
