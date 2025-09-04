using UnityEngine;
using UnityEngine.AI;

public class ArcherStateManager : MonoBehaviour
{
    public ArcherBaseState currentState;
    public ArcherIdleState idleState = new ArcherIdleState();
    public ArcherAttackState attackState = new ArcherAttackState();
    public ArcherChaseState chaseState = new ArcherChaseState();

    [HideInInspector] public Transform targetPlayer;
    [HideInInspector] public Vector2 spawnLocation;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public bool playerPointFound;
    [HideInInspector] public Transform lockedTarget;

    public GameObject chaseRangeTrigger;
    public LayerMask obstacleLayer;
    public GameObject arrowPrefab;
    public float attackCooldown = 2.5f;

    void Awake()
    {
        spawnLocation = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        FindPlayerPoint();
        currentState = idleState;
        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(ArcherBaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    public void OnRangeTriggerEnter(string rangeType, Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        targetPlayer = other.transform;
        if (rangeType == "Chase")
        {
            SwitchState(chaseState);
        }

        if (rangeType == "Shoot")
        {
            chaseRangeTrigger.SetActive(false);
            SwitchState(attackState);
        }
    }

    public void OnRangeTriggerExit(string rangeType, Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (rangeType == "Shoot")
        {
            chaseRangeTrigger.SetActive(true);
            SwitchState(chaseState);
        }

        if (rangeType == "Chase")
        {
            targetPlayer = null;
            SwitchState(idleState);
        }
    }

    public void FlipSprite()
    {
        float xDir = 0f;
        if (currentState == attackState && targetPlayer != null)
        {
            xDir = targetPlayer.position.x - transform.position.x;
        }
        else
        {
            xDir = agent.velocity.x;
        }

        if (Mathf.Abs(xDir) > 0.01)
        {
            spriteRenderer.flipX = xDir < 0f;
        }
    }

    public bool LineOfSight(Transform target)
    {
        Vector2 origin = transform.position;
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(origin, target.position);
        Debug.DrawRay(origin, direction.normalized * distance, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayer);
        return hit.collider == null;
    }
    
    public void FindPlayerPoint()
    {
        if (!playerPointFound)
        {
            Transform point = PlayerPointsManager.Instance?.GetRandomPlayerPoint();
            if (point != null)
            {
                lockedTarget = point;
                playerPointFound = true;
            }
        }
    }
}
