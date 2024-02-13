using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [Dropdown("GetRooms")]
    public Room Room;

    [SerializeField] Range<float> _spawnRange;
    int _enemiesToSpawn;
    float _spawnInterval = 5;

    public int SpawnEnemies(List<RoomSpawnable> enemies)
    {
        int maxSpawn = Room.CurrentWave * Mathf.Max(GameManager.Instance.PlayerScore / 100, 1);
        _enemiesToSpawn = maxSpawn;
        StartCoroutine(StartSpawning(enemies));
        return maxSpawn;
    }

    IEnumerator StartSpawning(List<RoomSpawnable> enemies)
    {
        while (_enemiesToSpawn > 0)
        {
            foreach (var enemy in enemies)
            {
                var dice = Random.Range(0, 100);
                if (dice > enemy.Chance) continue;

                SpawnEnemy(enemy.EnemyToSpawn);
            }
            _enemiesToSpawn--;
            yield return new WaitForSeconds(_spawnInterval);
        }
        _spawnInterval *= 0.9f;
    }

    void SpawnEnemy(GameObject enemy)
    {
        var size = transform.localScale / 2;
        // TODO replace 1 with 0 once using actual models
        var offset = 1;
        var loc = new Vector3(transform.localPosition.x + Random.Range(-size.x, size.x), transform.localPosition.y + offset, transform.localPosition.z + Random.Range(-size.z, size.z));

        var e = Instantiate(enemy, loc, Quaternion.identity).GetComponent<Enemy>();
        e.OnDie += OnEnemyDie;
    }

    void OnEnemyDie(LivingBeing e)
    {
        Room.DecreaseEnemy();
        e.OnDie -= OnEnemyDie;
    }

    public DropdownList<Room> GetRooms() => Room.GetRoomValues();
}
