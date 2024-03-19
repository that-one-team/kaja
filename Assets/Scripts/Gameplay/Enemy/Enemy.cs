using System.Collections;
using NaughtyAttributes;
using TOT.Common;
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
    protected AudioSource Audio;

    protected Transform Visuals;

    protected SpritesheetAnimation Anim;

    [Header("FX")]
    [SerializeField] GameObject _itemDropPrefab;
    [SerializeField] AudioClip[] _idleSfx;
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
        Visuals = transform.GetChild(0);
    }

    private void OnEnable()
    {
        MaxHealth = Data.StartingHealth;
        Health = MaxHealth;
    }

    private void Awake()
    {
        OnValidate();
        Anim = GetComponent<SpritesheetAnimation>();
        Audio = GetComponent<AudioSource>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = Data.MoveSpeed;
        Agent.angularSpeed = 0;

        OnHealthChanged += OnHurtEvent;
        RB.isKinematic = true;

        Target = PlayerController.Instance.transform; // :middle_finger:
        ChangeState(EnemyState.MOVING);
    }

    void OnHurtEvent(int changed, int remainingHealth)
    {
        ChangeState(EnemyState.STUNNED);
        if (_hurtGib != null && _gruntsSfx.Length > 0)
            Instantiate(_hurtGib, transform.position + Vector3.up, Quaternion.LookRotation(transform.forward)).GetComponent<GibVFX>().DoGib(_gruntsSfx.SelectRandom());
    }

    public override void Die()
    {
        if (_deathGib != null)
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

    protected void SetState(IEnumerator routine)
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
        SetSubroutine(PlayRandomIdle());
        Anim.SetAnimation(AnimationIndex.MOVE);
        Freeze(false);
        while (!IsInAttackRange())
        {
            Agent.SetDestination(Target.position);
            yield return new WaitForEndOfFrame();
        }
        Freeze(true);
        yield return new WaitForSeconds(Data.AttackDelay);
        ChangeState(IsInAttackRange() ? EnemyState.ATTACKING : EnemyState.MOVING);
    }

    IEnumerator PlayRandomIdle()
    {
        if (_idleSfx.Length == 0) yield return null;
        yield return new WaitForSeconds(Random.value * 10);
        Audio.PlayOneShot(_idleSfx.SelectRandom());
    }

    protected virtual IEnumerator StunnedState()
    {
        Freeze(true);

        Anim.SetAnimation(AnimationIndex.STUNNED);
        yield return new WaitForSeconds(Data.StunDuration);
        Freeze(false);
        Anim.SetAnimation(AnimationIndex.MOVE);
        ChangeState(EnemyState.MOVING);
    }

    protected virtual IEnumerator AttackState()
    {
        Anim.SetAnimation(AnimationIndex.ATTACK);
        SetSubroutine(Data.AttackType == EnemyAttackType.MELEE ? AttackMelee() : AttackRanged());
        yield return new WaitForSeconds(Data.FireRate);
        ChangeState(EnemyState.MOVING);
    }

    protected virtual IEnumerator AttackMelee()
    {
        yield return new WaitForSeconds(Data.AttackDelay);
        var pos = transform.position + Visuals.forward + Vector3.up;
        if (Physics.Raycast(transform.position + transform.up, Visuals.forward, out RaycastHit hit, 3, _attackLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<Player>().Damage(Data.Damage);
                pos = hit.point - (Visuals.forward * 0.9f);
            }
        }
        Instantiate(_attackGib, pos, Quaternion.LookRotation(Visuals.forward)).GetComponent<GibVFX>().DoGib(_attackSfx);
    }

    protected virtual IEnumerator AttackRanged()
    {
        yield break;
    }

    protected void Freeze(bool isFrozen)
    {
        RB.isKinematic = !isFrozen;
        // Agent.enabled = !isFrozen;
        Agent.isStopped = isFrozen;
        Anim.IsFrozen = isFrozen;
    }

    protected bool IsInAttackRange()
    {
        var trg = Target.position;
        trg.y = 0;
        var me = transform.position;
        me.y = 0;
        return Vector3.Distance(me, trg) < Data.AttackRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Visuals.forward + transform.up, _attackBoxSize);
    }
}