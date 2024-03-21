using System.Collections.Generic;
using TOT.Common;
using UnityEngine;

public class AmmoDrops : MonoBehaviour
{
    [SerializeField] bool _spawnInWorld = false;
    [SerializeField] List<GameObject> _ammoDrops;

    private void Start()
    {
        if (!_spawnInWorld)
            SpawnItems();
        WorldManager.Instance.OnWorldChange += WorldChange;
    }

    private void OnDisable()
    {
        WorldManager.Instance.OnWorldChange -= WorldChange;
    }

    private void WorldChange(WorldBrain brain)
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        int spawn = Random.Range(1, _ammoDrops.Count);
        for (int i = 0; i < spawn; i++)
        {
            var toSpawn = _ammoDrops.SelectRandom();
            Instantiate(toSpawn, transform.position, Quaternion.identity);
        }

        if (!_spawnInWorld)
            Destroy(gameObject);
    }
}
