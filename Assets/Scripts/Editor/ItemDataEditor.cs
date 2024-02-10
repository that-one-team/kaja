using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var item = (ItemData)target;

        if (GUILayout.Button("Generate Item"))
        {
            var prefabsPath = $"Assets/Resources/Prefabs/Items/{item.FriendlyName}.prefab";
            prefabsPath = AssetDatabase.GenerateUniqueAssetPath(prefabsPath);

            var spawned = ItemGod.SpawnItem(item, Vector3.one);
            Selection.activeGameObject = PrefabUtility.SaveAsPrefabAssetAndConnect(spawned, prefabsPath, InteractionMode.UserAction);
            SceneView.FrameLastActiveSceneView(); // focus on object

            AssetDatabase.SaveAssets();
        }
    }
}