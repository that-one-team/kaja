using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public bool IsPaused { get; private set; }

    public void PauseGame(bool isPaused = false)
    {
        IsPaused = isPaused;

        Time.timeScale = isPaused ? 0 : 1;
    }

}
