using UnityEngine;

public enum EnemyAttackType
{
    MELEE,
    RANGED
}

[CreateAssetMenu(fileName = "Enemy Data", menuName = "proj/Enemy Data", order = 0)]
public class EnemyData : ScriptableObject
{
    [Header("General")]
    public int StartingHealth;
    public string FriendlyName;
    public Texture Spritesheet;
    public int ScoreRequirement = 0;
    public int ScoreToGive = 10;
    public float MoveSpeed;

    [Header("Attacks")]
    public EnemyAttackType AttackType;
    public int Damage = 10;
    public float AttackRange = 3f;
    public float AttackDelay = 0.5f;
    public float FireRate = 1;
    public float StunDuration = 1;
}