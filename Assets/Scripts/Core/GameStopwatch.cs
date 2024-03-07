using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStopwatch : SingletonBehaviour<GameStopwatch>
{
    public bool IsActive;
    public float CurrentTime { get; private set; } = 1;

    // Update is called once per frame
    void Update()
    {
        if (!IsActive) return;

        CurrentTime += Time.deltaTime;
    }
}
