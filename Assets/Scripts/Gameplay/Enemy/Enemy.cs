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

[RequireComponent(typeof(SpritesheetAnimation))]
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
    Coroutine _stateSubroutine;

    protected NavMeshAgent Agent;
    protected Rigidbody RB;

    protected Transform Target;
    AudioSource _audio;

    Transform _visual;

    SpritesheetAnimation _anim;

    [Header("FX")]
    [SerializeField] GameObject _itemDropPrefab;
    [SerializeField] AudioClip[] _gruntsSfx;
    [SerializeField] AudioClip[] _deathSfx;
    [SerializeField] AudioClip _attackSfx;
    [SerializeField] GameObject _attackGib;
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
        _anim = GetComponent<SpritesheetAnimation>();
        _audio = GetComponent<AudioSource>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = Data.MoveSpeed;
        Agent.angularSpeed = 0;

        OnHurt += OnHurtEvent;

        RB.isKinematic = true;
        Target = PlayerController.Instance.transform; // :middle_finger:
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
        PlayerScore.Instance.AddScore(Mathf.RoundToInt(Data.ScoreToGive * Mathf.Pow(1, GameStopwatch.Instance.CurrentTime)));

        var dropChance = Random.value;
        if (dropChance < 0.7f)
            Instantiate(_itemDropPrefab, transform.position, Quaternion.identity);
        base.Die();
    }

    public void Knockback(Vector3 dir)
    {
        RB.AddForce(dir, ForceMode.Impulse);
    }

    protected void ChangeState(EnemyState state)
    {
        // print("[Enemy]: Changed state to " + state);
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
        SetSubroutine(null);

        _currentStateRoutine = StartCoroutine(routine);
    }

    void SetSubroutine(IEnumerator routine)
    {
        if (_stateSubroutine != null)
        {
            StopCoroutine(_stateSubroutine);
            _stateSubroutine = null;
        }
        if (routine != null)
            _stateSubroutine = StartCoroutine(routine);
    }

    protected virtual IEnumerator MoveState()
    {
        _anim.SetAnimation(AnimationIndex.MOVE);
        Freeze(false);
        while (Vector3.Distance(transform.position, Target.position) > 3f)
        {
            Agent.SetDestination(Target.position);
            yield return new WaitForEndOfFrame();
        }
        Freeze(true);
        ChangeState(EnemyState.ATTACKING);
    }

    protected virtual IEnumerator StunnedState()
    {
        Freeze(true);

        _anim.SetAnimation(AnimationIndex.STUNNED);
        yield return new WaitForSeconds(Data.StunDuration);
        Freeze(false);
        _anim.SetAnimation(AnimationIndex.MOVE);
        ChangeState(EnemyState.MOVING);
    }

    protected virtual IEnumerator AttackState()
    {
        _anim.SetAnimation(AnimationIndex.ATTACK);
        SetSubroutine(Data.AttackType == EnemyAttackType.MELEE ? AttackMelee() : AttackRanged());
        yield return new WaitForSeconds(Data.FireRate);
        ChangeState(EnemyState.MOVING);
    }

    protected virtual IEnumerator AttackMelee()
    {
        yield return new WaitForSeconds(Data.AttackDelay);
        var pos = transform.position + _visual.forward + Vector3.up;
        if (Physics.Raycast(transform.position + transform.up, _visual.forward, out RaycastHit hit, 3, _attackLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<Player>().Damage(Data.Damage);
                pos = hit.point - (_visual.forward * 0.9f);
            }
        }
        Instantiate(_attackGib, pos, Quaternion.LookRotation(_visual.forward)).GetComponent<GibVFX>().DoGib(_attackSfx);
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