using UnityEngine;

public class VickxItem : MonoBehaviour
{
    Player _player;
    float _timer;
    private void Start()
    {
        _player = Player.Instance;
        _timer = 5;
    }

    private void Update()
    {
        if (_player.Health == _player.MaxHealth) return;

    }
}