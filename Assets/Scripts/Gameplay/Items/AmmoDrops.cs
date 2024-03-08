using System.Collections.Generic;
using TOT.Common;
using UnityEngine;

public class AmmoDrops : MonoBehaviour
{
    [SerializeField] List<GameObject> _ammoDrops;

    private void Start()
    {
        int spawn = Random.Range(1, _ammoDrops.Count);
        for (int i = 0; i < spawn; i++)
        {
            var toSpawn = _ammoDrops.SelectRandom();
            Instantiate(toSpawn, transform.position, Quaternion.identity);
        }
    }
}
