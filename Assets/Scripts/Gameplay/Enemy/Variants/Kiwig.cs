using System.Collections;
using UnityEngine;

public class Kiwig : Enemy
{
    readonly WaitForEndOfFrame _wait = new();

    protected override IEnumerator MoveState()
    {
        Freeze(false);
        // Agent.isStopped = false;
        while (Vector3.Distance(transform.position, Target.position) > 3f)
        {
            Agent.SetDestination(Target.position);
            yield return _wait;
        }
        Freeze(true);
        // Agent.isStopped = true;
        ChangeState(EnemyState.ATTACKING);
    }

}
