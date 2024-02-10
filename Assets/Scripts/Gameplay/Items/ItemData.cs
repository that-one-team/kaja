using UnityEngine;
using NaughtyAttributes;

public enum ItemType
{
    ITEM,
    WEAPON
}

public enum ItemInteractionType
{
    LOOK_AT,
    COLLIDE
}

[CreateAssetMenu(fileName = "Item Data", menuName = "proj/Item Data", order = 0)]
public class ItemData : ScriptableObject
{
    [Header("Item settings")]
    public string FriendlyName = "Item";
    public ItemType Type;
    public ItemInteractionType InteractionType = ItemInteractionType.COLLIDE;
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

    [Header("Weapon settings")]

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public float FireRate = 0.5f;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public bool IsProjectile = false;

    [ShowIf("IsProjectile")]
    [AllowNesting]
    public GameObject ProjectilePrefab;

}