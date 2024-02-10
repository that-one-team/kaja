using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public bool IsPaused { get; private set; }
    public bool IsFrozen { get; set; }

    public void PauseGame(bool isPaused = false)
    {
        IsPaused = isPaused;
        IsFrozen = isPaused;

        Time.timeScale = isPaused ? 0 : 1;
    }

}
