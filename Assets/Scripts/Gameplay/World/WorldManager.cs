using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TOT.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : SingletonBehaviour<WorldManager>
{
    public WorldBrain CurrentWorld { get; private set; }
    public static event Action<WorldBrain> OnWorldChange;

    [SerializeField][Scene] List<string> _worldsToLoad;
    readonly Queue<string> _worldPool = new();

    [Scene]
    [SerializeField] string _startingWorld;
    [SerializeField] bool DEBUG_LoadRooms = false;

    public List<WorldBrain> WorldPool { get; private set; }

    protected override void PostAwake()
    {
        print("world manager");
#if UNITY_EDITOR
        if (!DEBUG_LoadRooms) return;
#endif
        var load = SceneManager.LoadSceneAsync(_startingWorld, LoadSceneMode.Additive);
        load.completed += (AsyncOperation op) =>
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_startingWorld));

        LoadOtherWorlds();
    }

    void LoadOtherWorlds()
    {
        var shuffled = _worldsToLoad.Shuffle();
        foreach (var world in shuffled) _worldPool.Enqueue(world);
        LoadNextWorld();
    }

    public void LoadNextWorld()
    {
        SceneManager.LoadSceneAsync(_worldPool.Peek(), LoadSceneMode.Additive);
    }

    public void SetWorld(WorldBrain world)
    {
        CurrentWorld = world;
        OnWorldChange?.Invoke(world);
    }
}
