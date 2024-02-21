using System.Collections;
using NaughtyAttributes;
using TOT.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    MOVING,
    ATTACKING,
    STUNNED
}

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : LivingBeing
{
    [Expandable]
    public EnemyData Data;

    [SerializeField] float _attackBoxSize = 1;
    [SerializeField] LayerMask _attackLayer;

    public EnemyState CurrentState { get; protected set; }
    Coroutine _currentStateRoutine;

    protected NavMeshAgent Agent;
    protected Rigidbody RB;

    protected Transform Target;
    AudioSource _source;

    Transform _visual;

    [Header("FX")]
    [SerializeField] AudioClip[] _gruntsSfx;
    [SerializeField] AudioClip[] _deathSfx;
    [SerializeField] GameObject _deathGib;
    [SerializeField] GameObject _hurtGib;

    private void OnValidate()
    {
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        RB.mass = 2;
        _visual = transform.GetChild(0);
    }

    private void Awake()
    {
        OnValidate();
        _source = GetComponent<AudioSource>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = Data.MoveSpeed;
        Agent.angularSpeed = 0;

        OnHurt += OnHurtEvent;

        RB.isKinematic = true;
        Target = GameManager.Instance.GetPlayer();
        ChangeState(EnemyState.MOVING);
    }

    void OnHurtEvent(int remainingHealth)
    {
        ChangeState(EnemyState.STUNNED);
        if (_hurtGib != null)
            Instantiate(_hurtGib, transform.position + Vector3.up, Quaternion.LookRotation(transform.forward)).GetComponent<GibVFX>().DoGib(_gruntsSfx.SelectRandom());
    }

    public override void Die()
    {
        Instantiate(_deathGib, transform.position + Vector3.up, Quaternion.identity).GetComponent<GibVFX>().DoGib(_deathSfx.SelectRandom());
        base.Die();
    }

    public void Knockback(Vector3 dir)
    {
        RB.AddForce(dir, ForceMode.Impulse);
    }

    protected void ChangeState(EnemyState state)
    {
        print("[Enemy]: Changed state to " + state);
        switch (state)
        {
            case EnemyState.MOVING:
                SetState(MoveState());
                break;
            case EnemyState.ATTACKING:
                SetState(AttackState());
                break;
            case EnemyState.STUNNED:
                SetState(StunnedState());
                break;

        }
    }

    void SetState(IEnumerator routine)
    {
        if (_currentStateRoutine != null)
        {
            StopCoroutine(_currentStateRoutine);
            _currentStateRoutine = null;
        }

        _currentStateRoutine = StartCoroutine(routine);
    }

    protected virtual IEnumerator MoveState()
    {
        yield break;
    }

    protected virtual IEnumerator StunnedState()
    {
        Freeze(true);

        // TODO switch to stunned spritesheet
        yield return new WaitForSeconds(Data.StunDuration);
        Freeze(false);
        ChangeState(EnemyState.MOVING);
    }

    protected virtual IEnumerator AttackState()
    {
        StartCoroutine(Data.AttackType == EnemyAttackType.MELEE ? AttackMelee() : AttackRanged());
        yield return new WaitForSeconds(Data.FireRate);
        ChangeState(EnemyState.MOVING);
    }

    protected virtual IEnumerator AttackMelee()
    {
        yield return new WaitForSeconds(Data.AttackDelay);
        if (Physics.Raycast(transform.position + transform.up, _visual.forward, out RaycastHit hit, 3, _attackLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<Player>().Damage(Data.Damage);
            }
        }
    }

    protected virtual IEnumerator AttackRanged()
    {
        yield break;
    }

    protected void Freeze(bool isFrozen)
    {
        RB.isKinematic = !isFrozen;
        Agent.enabled = !isFrozen;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _visual.forward + transform.up, _attackBoxSize);
    }
}