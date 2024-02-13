using UnityEngine;

public enum EnemyAttackType
{
    MELEE,
    RANGED
}

[CreateAssetMenu(fileName = "Enemy Data", menuName = "proj/Enemy Data", order = 0)]
public class EnemyData : ScriptableObject
{
    public string FriendlyName;
    public Texture Spritesheet;
    public float MoveSpeed;
    public EnemyAttackType AttackType;

    [Tooltip("Does not run away after using certain skill")]
    public float Pride;
}