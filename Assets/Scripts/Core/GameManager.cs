using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public bool IsPaused { get; private set; }
    public bool IsFrozen { get; set; }

    public Room ActiveRoom { get; private set; }
    public string CurrentWorld { get; set; }

    public event System.Action<Room> OnChangeRoom;

    public int PlayerScore { get; private set; } = 0;

    public void PauseGame(bool isPaused = false)
    {
        IsPaused = isPaused;
        IsFrozen = isPaused;

        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ChangeRoom(Room room)
    {
        ActiveRoom = room;
        OnChangeRoom?.Invoke(room);
    }

}
