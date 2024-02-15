using System.Collections;
using UnityEngine;

public class Kiwig : Enemy
{
    readonly WaitForEndOfFrame _wait = new();

    protected override IEnumerator MoveState()
    {
        Agent.isStopped = false;
        while (Vector3.Distance(transform.position, Target.position) > 3f)
        {
            Agent.SetDestination(Target.position);
            yield return _wait;
        }
        Agent.isStopped = true;
        ChangeState(EnemyState.ATTACKING);
    }

    protected override IEnumerator AttackState()
    {
        print("attacking player");
        yield return new WaitForSeconds(2);
        ChangeState(EnemyState.MOVING);
    }
}
