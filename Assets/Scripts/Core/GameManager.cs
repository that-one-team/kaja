using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField] GameObject _pauseMenu;
    public bool IsPaused { get; private set; }
    public bool IsFrozen { get; set; }

    Transform _player;
    void Start()
    {
        Freeze(false);
        PlayerInputs.Instance.UI.Pause.performed += OnPausePressed;
        PauseGame(false);
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.UI.Pause.performed -= OnPausePressed;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        PauseGame(!IsPaused);
    }

    public void PauseGame(bool isPaused = false)
    {
        IsPaused = isPaused;
        IsFrozen = isPaused;

        Time.timeScale = isPaused ? 0 : 1;

        _pauseMenu.SetActive(isPaused);
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
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

    public void Forfeit()
    {
        GameStopwatch.Instance.IsActive = false;
        WorldManager.Instance.ResetWorldPool();
        PauseGame(false);
    }

}
