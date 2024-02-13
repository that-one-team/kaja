using UnityEngine;
using NaughtyAttributes;
using UnityEditor;
using System.IO;

public enum ItemType
{
    ITEM,
    WEAPON
}

[CreateAssetMenu(fileName = "Item Data", menuName = "proj/Item Data", order = 0)]
public class ItemData : ScriptableObject
{
    [Header("Item settings")]
    public string FriendlyName = "Item";
    public ItemType Type;
    public InteractionType InteractionType = InteractionType.COLLIDE;
    public bool IsStackable = false;

    [Header("Visuals")]
    public Texture2D WorldSprite;
    [ShowIf("Type", ItemType.WEAPON)]
    public Texture2D HandSprite;
    public float Scale = 1;
    public Color LightColor = Color.white;

    [Header("Audio")]
    public AudioClip PickupAudio;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public AudioClip EquipAudio;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public AudioClip ShootAudio;

    [Header("Weapon settings")]

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public float FireRate = 0.5f;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public int Damage = 20;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public bool IsProjectile = false;

    [ShowIf("IsProjectile")]
    [AllowNesting]
    public GameObject ProjectilePrefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
        FriendlyName = name;
    }
#endif
}