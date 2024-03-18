using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.ProBuilder;
public class EnemySpawner : MonoBehaviour
{
    [Dropdown("GetRooms")]
    public Room Room;

    [SerializeField] Range<float> _spawnRange;
    int _enemiesToSpawn;
    float _spawnInterval = 5;

    private void Start()
    {
        if (_spawnRange.Max == 0) _spawnRange = new(1, 1);
        if (Room == null) Room = GetComponentInParent<Room>();
    }

    public int SpawnEnemies(List<RoomSpawnable> enemies)
    {
        int maxSpawn = Mathf.RoundToInt(_spawnRange.Max * Mathf.Pow(Room.CurrentWave, GameStopwatch.Instance.CurrentTime));

        print("Spawning " + maxSpawn + " enemies");

        _enemiesToSpawn = maxSpawn;
        StartCoroutine(StartSpawning(enemies));
        return maxSpawn;
    }

    IEnumerator StartSpawning(List<RoomSpawnable> enemies)
    {
        while (_enemiesToSpawn > 0)
        {
            var toSpawn = Mathf.Min(Random.Range(_spawnRange.Min / 2, _spawnRange.Min), _enemiesToSpawn);
            for (int i = 0; i < Mathf.CeilToInt(toSpawn); i++)
            {
                foreach (var enemy in enemies)
                {
                    var dice = Random.Range(0, 100);
                    if (dice > enemy.Chance) continue;

                    SpawnEnemy(enemy.EnemyToSpawn);
                }
            }

            _spawnInterval = Random.Range(_spawnInterval, _spawnInterval + 5);
            yield return new WaitForSeconds(_spawnInterval);
        }
        _spawnInterval *= 0.9f;
    }

    void SpawnEnemy(GameObject enemy)
    {
        var size = GetComponent<Renderer>().bounds.extents;
        var center = GetComponent<Renderer>().bounds.center;
        var offset = 1;
        var x = Random.Range(center.x - size.x, center.x + size.x);
        var z = Random.Range(center.z - size.z, center.z + size.z);
        var loc = new Vector3(x, transform.position.y + offset, z);

        var e = Instantiate(enemy, loc, Quaternion.identity).GetComponent<Enemy>();
        e.OnDie += OnEnemyDie;
        _enemiesToSpawn--;
    }

    void OnEnemyDie(LivingBeing e)
    {
        Room.DecreaseEnemy();
        e.OnDie -= OnEnemyDie;
    }

    public DropdownList<Room> GetRooms() => Room.GetRoomValues();
}
