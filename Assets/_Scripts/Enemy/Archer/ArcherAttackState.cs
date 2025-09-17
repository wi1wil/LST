using System.Collections;
using UnityEngine;

public class ArcherAttackState : ArcherBaseState
{
    Coroutine attackRoutine;

    public override void EnterState(ArcherStateManager archer)
    {
        archer.agent.isStopped = true;
        archer.animator.SetBool("isChasing", false);
        archer.animator.SetBool("isAttacking", true);

        if (attackRoutine == null)
        {
            attackRoutine = archer.StartCoroutine(AttackPlayer(archer));
        }
    }

    public override void UpdateState(ArcherStateManager archer)
    {
        if (archer.targetPlayer != null)
        {
            archer.FlipSprite();
        }
    }

    public override void ExitState(ArcherStateManager archer)
    {
        if (attackRoutine != null)
        {
            archer.StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        archer.animator.SetBool("isAttacking", false);
    }

    private IEnumerator AttackPlayer(ArcherStateManager archer)
    {
        while (archer.targetPlayer != null && archer.currentState is ArcherAttackState)
        {
            archer.animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(0.4f);

            GameObject arrow = Object.Instantiate(
                archer.arrowPrefab,
                archer.transform.position,
                Quaternion.identity
            );

            if (archer.targetPlayer != null)
            {
                ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
                arrowScript.Shoot(archer.targetPlayer.position, archer.transform.position);
            }

            archer.animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(archer.attackCooldown);
        }
        attackRoutine = null;
    }
}
