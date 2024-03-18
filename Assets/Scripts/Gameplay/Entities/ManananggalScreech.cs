using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManananggalScreech : Projectile
{
    void Update()
    {
        transform.localScale += Time.deltaTime * Vector3.one;
    }
}
