using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : SingletonBehaviour<PlayerScore>
{
    public int Score { get; private set; }

    public Action<int> OnAddScore;

    public void AddScore(int score)
    {
        Score += score;
        OnAddScore?.Invoke(Score);
    }
}
