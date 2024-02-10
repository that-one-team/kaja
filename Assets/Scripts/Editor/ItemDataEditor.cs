using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var item = (ItemData)target;

        if (GUILayout.Button("Generate Item"))
        {
            var spawned = ItemGod.SpawnItem(item, Vector3.one);
            Selection.activeGameObject = spawned;
            SceneView.FrameLastActiveSceneView(); // focus on object
        }
    }
}