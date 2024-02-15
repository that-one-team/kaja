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
    public int ScoreRequirement = 0;
    public float MoveSpeed;
    public EnemyAttackType AttackType;
    public float StunDuration = 1;
}