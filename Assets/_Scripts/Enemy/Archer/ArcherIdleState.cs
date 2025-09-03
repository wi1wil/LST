using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ArcherIdleState : ArcherBaseState
{
    private float roamTimer = 10f;
    private float roamCooldown = 10f;
    private Coroutine roamRoutine;

    public override void EnterState(ArcherStateManager archer)
    {
        archer.animator.SetBool("isChasing", false);
    }

    public override void UpdateState(ArcherStateManager archer)
    {
        if (!archer.agent.isStopped && archer.agent.remainingDistance > 0.1f)
        {
            return;
        }

        roamTimer -= Time.deltaTime;
        if (roamTimer <= 0f && roamRoutine == null)
        {
            roamRoutine = archer.StartCoroutine(InitializeRoaming(archer)); 
        }
    }

    public override void ExitState(ArcherStateManager archer)
    {

    }

    private IEnumerator InitializeRoaming(ArcherStateManager archer)
    {
        roamTimer = roamCooldown;

        Vector2 randomDir = Random.insideUnitCircle.normalized * Random.Range(1f, 2f);
        Vector2 newDir = archer.spawnLocation + randomDir;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newDir, out hit, 1f, NavMesh.AllAreas))
        {
            Debug.Log("Im gonna go roam!");
            archer.agent.isStopped = false;
            archer.animator.SetBool("isChasing", true);
            archer.agent.SetDestination(hit.position);

            while (archer.agent.pathPending || archer.agent.remainingDistance > 0.1f)
            {
                if (!(archer.currentState is ArcherIdleState)) yield break;

                archer.FlipSprite();
                yield return null;
            }
            Debug.Log("I reached ma roam location!");
            archer.agent.isStopped = true;
            archer.animator.SetBool("isChasing", false);
        }
        roamRoutine = null;
    }
}
