using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TOT.Common;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class WorldManager : SingletonBehaviour<WorldManager>
{
    [field: ShowNonSerializedField]
    public WorldBrain CurrentWorld { get; private set; }
    public event Action<WorldBrain> OnWorldChange;

    [SerializeField][Scene] List<string> _worldsToLoad;
    [SerializeField][Scene] List<string> _specialWorlds;
    readonly Queue<string> _worldPool = new();

    [Scene]
    [SerializeField] string _startingWorld;
    [SerializeField] bool DEBUG_LoadRooms = false;

    WorldBrain _nextWorld;

    protected override void PostAwake()
    {
#if UNITY_EDITOR
        if (!DEBUG_LoadRooms) return;
#endif

        ResetWorldPool();
    }

    public void ResetWorldPool()
    {
        // Clear spawned worlds
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            var scn = SceneManager.GetSceneAt(i);
            if (scn.name == "SCN_Core") continue;
            SceneManager.UnloadSceneAsync(scn);
        }
        _worldPool.Clear();

        var load = SceneManager.LoadSceneAsync(_startingWorld, LoadSceneMode.Additive);
        load.completed += (AsyncOperation op) =>
            {
                var scn = SceneManager.GetSceneByName(_startingWorld);
                SceneManager.SetActiveScene(scn);
            };

        LoadOtherWorlds();
    }

    public void LoadOtherWorlds()
    {
        var shuffled = _worldsToLoad.Shuffle();
        foreach (var world in shuffled) _worldPool.Enqueue(world);
        _worldPool.Enqueue(_specialWorlds.SelectRandom());
    }

    public void LoadNextWorld()
    {
        var tryRemove = _worldPool.TryDequeue(out string next);
        if (!tryRemove)
        {
            LoadOtherWorlds();
            LoadNextWorld(); // call func again
            return;
        }

        var load = SceneManager.LoadSceneAsync(next, LoadSceneMode.Additive);

        load.completed += (AsyncOperation op) =>
        {
            foreach (var obj in SceneManager.GetSceneByName(next).GetRootGameObjects())
            {
                if (obj.TryGetComponent(out WorldBrain brain))
                {
                    _nextWorld = brain;
                }
            }
        };
    }

    public void UnloadPreviousWorld()
    {
        if (CurrentWorld != null)
            SceneManager.UnloadSceneAsync(CurrentWorld.SceneName);
    }

    public void NextWorld()
    {
        _nextWorld.PlayerSpawnPoint.TeleportPlayer();
        SceneManager.MoveGameObjectToScene(GameManager.Instance.GetPlayer().gameObject, _nextWorld.Scene);
        GameStopwatch.Instance.IsActive = !(_nextWorld.Scene.name == "SCN_World_Hub");
        ChangeWorld(_nextWorld);
    }

    public void ChangeWorld(WorldBrain world, Action onWorldChange = null)
    {
        UnloadPreviousWorld();
        CurrentWorld = world;
        if (_nextWorld != null)
            SceneManager.SetActiveScene(_nextWorld.Scene);

        onWorldChange?.Invoke();
        OnWorldChange?.Invoke(world);
    }

}
