using UnityEngine;
using System.Collections;

public class ArcherChaseState : ArcherBaseState
{
    public override void EnterState(ArcherStateManager archer)
    {
        archer.agent.isStopped = false;
        archer.animator.SetBool("isChasing", true);
    }

    public override void UpdateState(ArcherStateManager archer)
    {
        if (archer.lockedTarget == null) archer.FindPlayerPoint();

        archer.agent.SetDestination(archer.lockedTarget.position);
        archer.FlipSprite();
    }

    public override void ExitState(ArcherStateManager archer)
    {        
        archer.StartCoroutine(FinishPath(archer));
    }

    private IEnumerator FinishPath(ArcherStateManager archer)
    {
        while (archer.agent.pathPending || archer.agent.remainingDistance > 0.1f)
        {
            if(!(archer.currentState is ArcherChaseState)) yield break;

            archer.animator.SetBool("isChasing", true);
            yield return null;
        }

        archer.animator.SetBool("isChasing", false);
        archer.agent.isStopped = true;
        archer.agent.ResetPath();
    }
}
