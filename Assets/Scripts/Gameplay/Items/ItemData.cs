using UnityEngine;
using NaughtyAttributes;
using UnityEditor;
using System.IO;

public enum ItemType
{
    ITEM,
    WEAPON
}

// !WARNING: I DIDNT USE SEPARATE STRUCTS FOR WEAPON SETTINGS BECAUSE I AM LAZY :D
// glhf looking at this monstrosity (only because i used NaughtyAttributes so a designer (was supposed to be Aaron) who will implement weapons will have easy time)
// i ended up being the said designer.........
// its ok it looks cool in-editor
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
    [AllowNesting]
    public Texture2D HandSprite;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public Vector3 Offset;
    public float Scale = 1;
    public Color LightColor = Color.white;

    [ShowIf("Type", ItemType.WEAPON)]
    [Header("Animations")]
    [AllowNesting]
    public Vector3 EndPose = new(0.2f, -0.2f, 0);

    public GameObject UseItemVFX;

    [Header("Audio")]
    public AudioClip PickupAudio;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public AudioClip EquipAudio;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public AudioClip ShootAudio;

    [ShowIf("Type", ItemType.WEAPON)]
    [Header("Weapon settings")]
    [AllowNesting]
    public float FireRate = 0.5f;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public int InitialAmmo = 10;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public int Damage = 20;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public float KnockbackForce = 10;

    [ShowIf("Type", ItemType.WEAPON)]
    [AllowNesting]
    public bool IsProjectile = false;

    [ShowIf("IsProjectile")]
    [AllowNesting]
    public GameObject ProjectilePrefab;

    [ShowIf("IsProjectile")]
    [AllowNesting]
    public float ProjectileForce;

#if UNITY_EDITOR
    private void OnValidate()
    {
        name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
        FriendlyName = name;
    }
#endif
}
