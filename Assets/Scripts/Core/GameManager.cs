using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public bool IsPaused { get; private set; }
    public bool IsFrozen { get; set; }

    public int PlayerScore { get; private set; } = 0;

    Transform _player;

    private void Start()
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
            _player = GameObject.Find("Player").transform;
        }

        return _player;
    }

    public void Freeze(bool isFrozen)
    {
        IsFrozen = isFrozen;
        Cursor.lockState = IsFrozen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsFrozen;
    }

}
