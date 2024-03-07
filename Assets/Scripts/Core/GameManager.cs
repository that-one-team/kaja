using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public bool IsPaused { get; private set; }
    public bool IsFrozen { get; set; }

    Transform _player;

    void Start()
    {
        Freeze(false);
    }

    public void PauseGame(bool isPaused = false)
    {
        IsPaused = isPaused;
        IsFrozen = isPaused;

        Time.timeScale = isPaused ? 0 : 1;
    }

    public Transform GetPlayer()
    {
        if (_player == null)
        {
            _player = GameObject.Find("@Player(Clone)").transform;
        }

        return _player;
    }

    public Transform SetPlayer(Transform player) => _player = player;

    public void Freeze(bool isFrozen)
    {
        IsFrozen = isFrozen;
        Cursor.lockState = IsFrozen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsFrozen;
    }

}
