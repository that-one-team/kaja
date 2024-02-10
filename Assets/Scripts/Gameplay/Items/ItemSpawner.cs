using UnityEditor;
using UnityEngine;

public static class ItemGod
{
    public static GameObject SpawnItem(ItemData item, Vector3 position)
    {
        GameObject spawned = GameObject.CreatePrimitive(PrimitiveType.Quad);

        var itemMat = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Items/MAT_ItemBase.mat", typeof(Material));
        var renderer = spawned.GetComponent<Renderer>();
        renderer.SetMaterials(new System.Collections.Generic.List<Material>() { itemMat });
        var mat = renderer.material;
        mat.mainTexture = item.WorldSprite;


        var behavior = spawned.AddComponent<Item>();
        behavior.Data = item;

        spawned.name = item.name;
        spawned.transform.position = position;

        var light = spawned.AddComponent<Light>();
        light.intensity = 5;

        return spawned;
    }
}

public class ItemSpawner : SingletonBehaviour<ItemSpawner>
{
    public void SpawnItem(ItemData item, Vector3 position)
    {
        ItemGod.SpawnItem(item, position);
    }
}