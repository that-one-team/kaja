using UnityEngine;
using UnityEditor;

public class ToolsWindow : EditorWindow
{
    static Material _spawnerMaterial, _roomVolumeMaterial;

    [MenuItem("TOT/Tools")]
    private static void ShowWindow()
    {
        ReloadAssets();
        var window = GetWindow<ToolsWindow>();
        window.titleContent = new GUIContent("that-one-team tools");
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Reload assets")) ReloadAssets();
        GUILayout.Space(10);

        var selected = Selection.activeGameObject;
        ConvertToRoomVolume(selected);

        GUILayout.Space(10);

        PlaceEnemySpawner();
        ConvertToEnemySpawner(selected);
    }

    static void ReloadAssets()
    {
        _spawnerMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Volumes/VOL_Spawner.mat", typeof(Material));
        _roomVolumeMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Volumes/VOL_Room.mat", typeof(Material));
    }

    void PlaceEnemySpawner()
    {
        if (GUILayout.Button("Create Enemy spawner"))
        {
            var spawner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UpdateSpawnerInfo(spawner);
        }
    }

    void ConvertToEnemySpawner(GameObject selected)
    {
        if (GUILayout.Button("Convert to Enemy spawner"))
        {
            if (selected == null) return;
            UpdateSpawnerInfo(selected);
        }
    }

    void UpdateSpawnerInfo(GameObject spawner)
    {
        spawner = UpdateVolume(spawner, "V_EnemySpawnZone", _spawnerMaterial);
        if (!spawner.TryGetComponent<EnemySpawner>(out _))
            spawner.AddComponent<EnemySpawner>();
    }

    GameObject UpdateVolume(GameObject volume, string volumeName, Material material)
    {
        volume.name = volumeName;
        volume.layer = 1;
        volume.GetComponent<Renderer>().material = material;
        if (volume.TryGetComponent(out MeshCollider col))
            col.convex = true;

        volume.GetComponent<Collider>().isTrigger = true;
        Selection.activeGameObject = volume;
        SceneView.FrameLastActiveSceneView();

        return volume;
    }
    void ConvertToRoomVolume(GameObject volume)
    {
        if (GUILayout.Button("Convert to room volume"))
        {
            if (volume == null) return;
            volume = UpdateVolume(volume, "V_Room entrance volume", _roomVolumeMaterial);
            if (!volume.TryGetComponent<RoomVolume>(out _))
                volume.AddComponent<RoomVolume>();

            if (volume.transform.childCount == 0)
            {
                var blockerPref = Resources.Load("Prefabs/Static Mesh/DoorBlocker", typeof(GameObject));
                var blocker = Instantiate(blockerPref, volume.transform) as GameObject;
                blocker.transform.position = volume.transform.position;
                blocker.SetActive(false);
            }
        }
    }
}