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

    public EnemyState CurrentState { get; protected set; }
    Coroutine _currentStateRoutine;

    protected NavMeshAgent Agent;
    protected Rigidbody RB;

    protected Transform Target;

    AudioSource _source;

    [Header("FX")]
    [SerializeField] AudioClip[] _gruntsSfx;
    [SerializeField] AudioClip[] _deathSfx;
    [SerializeField] GameObject _deathGib;

    private void OnValidate()
    {
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        RB.mass = 2;
    }

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        RB = GetComponent<Rigidbody>();
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
        _source.PlayOneShot(_gruntsSfx.SelectRandom());
    }

    public override void Die()
    {
        Instantiate(_deathGib, transform.position, Quaternion.identity).GetComponent<DeathGibVFX>().DoGib(_deathSfx.SelectRandom());
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

    protected virtual IEnumerator AttackState()
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

    void Freeze(bool isFrozen)
    {
        RB.isKinematic = !isFrozen;
        Agent.enabled = !isFrozen;
    }
}