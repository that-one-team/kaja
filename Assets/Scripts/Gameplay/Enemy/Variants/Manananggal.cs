using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manananggal : Enemy
{
    [SerializeField] float _minHeight;
    [SerializeField] GameObject _screechProjectile;
    [SerializeField] float _predictionOffset = 1;

    protected override IEnumerator MoveState()
    {
        Agent.baseOffset = _minHeight;
        Anim.SetAnimation(AnimationIndex.MOVE);
        Freeze(false);
        while (!IsInAttackRange())
        {
            var rnd = Random.insideUnitCircle;
            Agent.SetDestination(Target.position + new Vector3(rnd.x, 0, rnd.y));
            yield return new WaitForEndOfFrame();
        }
        Freeze(true);
        yield return new WaitForSeconds(Data.AttackDelay);
        ChangeState(EnemyState.ATTACKING);
    }

    protected override IEnumerator AttackRanged()
    {
        var targetVel = Target.GetComponent<Rigidbody>().velocity;
        targetVel.y = 0;
        var predictedPos = Target.position + (targetVel * _predictionOffset);
        var dir = predictedPos - transform.position;

        var projectile = Instantiate(_screechProjectile, transform.position + Visuals.forward + Vector3.up, Quaternion.LookRotation(dir));
        projectile.GetComponent<Projectile>().Spawn(Data.Damage, dir);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());
        yield return new WaitForEndOfFrame();
    }
}
