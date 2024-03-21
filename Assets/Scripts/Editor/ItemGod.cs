using UnityEditor;
using UnityEngine;

public class ItemGod
{
    public static GameObject SpawnItem(ItemData item, Vector3 position)
    {
        GameObject spawned = GameObject.CreatePrimitive(PrimitiveType.Quad);

        var itemMat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Items/MAT_ItemBase.mat", typeof(Material));
        var mat = new Material(itemMat);
        AssetDatabase.CreateAsset(mat, $"Assets/Materials/Items/MAT_Item_{item.FriendlyName}.mat");
        var renderer = spawned.GetComponent<Renderer>();
        renderer.sharedMaterial = mat;
        mat.mainTexture = item.WorldSprite;

        var behavior = spawned.AddComponent<Item>();
        behavior.Data = item;
        behavior.UpdateVisuals();

        spawned.name = item.name;
        spawned.transform.position = position;

        var light = spawned.AddComponent<Light>();
        light.intensity = 5;

        return spawned;
    }
}