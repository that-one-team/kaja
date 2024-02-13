using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingBeing
{
    [Expandable]
    public EnemyData Data;
}