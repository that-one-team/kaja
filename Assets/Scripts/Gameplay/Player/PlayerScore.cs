using System;
using NaughtyAttributes;
using UnityEngine;

public class PlayerScore : SingletonBehaviour<PlayerScore>
{
    [ShowNativeProperty]
    public int Score { get; private set; }

    public Action<int> OnAddScore;

    public void AddScore(int score)
    {
        Score += score;
        OnAddScore?.Invoke(Score);
    }
}
