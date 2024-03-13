using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] Vector3 _rotationSpeed;

    void Update()
    {
        transform.Rotate(_rotationSpeed * Time.deltaTime);
    }
}
