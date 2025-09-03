using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ArcherScript :  MonoBehaviour
{
    public float speed;
    public float shootingCooldown = 1f;

    public bool isChasing = false;
    public bool isShooting = false;
    public bool hasRecentlyShot = false;
    public bool playerDetected = false;
    public bool wayPointFound = false;

    public float chaseRange = 25f;
    public float detectionRange = 10f;
    public float avoidanceRange = 2.5f;
    public float avoidanceForce = 2f;

    public Transform topPoint;
    public Transform bottomPoint;
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform lockedTarget;
    public GameObject arrowPrefab;
    public LayerMask obstacleLayer;
    public Transform targetPlayer = null;

    Animator animator;
    NavMeshAgent agent;
    SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    void Awake()
    {
        GameObject top = GameObject.Find("Top");
        GameObject bottom = GameObject.Find("Bottom");
        GameObject left = GameObject.Find("Left");
        GameObject right = GameObject.Find("Right");

        topPoint = top.transform;
        bottomPoint = bottom.transform;
        leftPoint = left.transform;
        rightPoint = right.transform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    
    void Update()
    {
        DetectPlayer();
        UpdateBehavior();
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.black;
    //     Gizmos.DrawWireSphere(transform.position, detectionRange);
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, chaseRange);

    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawWireSphere(transform.position, avoidanceRange);

    //     if (targetPlayer != null)
    //     {
    //         Vector3 origin = transform.position;
    //         Vector3 target = targetPlayer.position;
    //         bool clear = LineOfSight(targetPlayer);

    //         Gizmos.color = clear ? Color.green : Color.yellow;
    //         Gizmos.DrawLine(origin, target);
    //     }
    // }

    public void DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, chaseRange, LayerMask.GetMask("Player"));
        bool foundPlayer = false;
        foreach (Collider2D hit in hits)
        {
            PlayerMovementScript player = hit.GetComponent<PlayerMovementScript>();
            if (player != null && !isShooting)
            {
                if (LineOfSight(player.transform))
                {
                    float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                    // Player detected within range
                    if(distanceToPlayer > detectionRange)
                        return;

                    targetPlayer = player.transform;
                    foundPlayer = true;
                    break; 
                }
                else if (playerDetected)
                {
                    targetPlayer = player.transform;
                    foundPlayer = true;
                    break; 
                }
            }
            if (!foundPlayer && targetPlayer != null)
            {
                targetPlayer = null; 
            }
        }
    }

    public bool LineOfSight(Transform target)
    {
        obstacleLayer = LayerMask.GetMask("Obstacle");

        Vector2 origin = transform.position;
        Vector2 direction = (Vector2)target.position - origin;
        float distance = Vector2.Distance(origin, target.position);
        RaycastHit2D hit = Physics2D.Raycast(origin, direction.normalized, distance, obstacleLayer);
        return hit.collider == null;
    }

    void UpdateBehavior()
    {        
        lockedTarget = findATarget();

        float distanceToPlayer = Vector2.Distance(transform.position, lockedTarget.position);
        // 1. Stop and shoot if close enough
        if (distanceToPlayer < detectionRange && !isShooting && !hasRecentlyShot && targetPlayer != null)
        {
            animator.SetBool("isChasing", false);
            agent.isStopped = true;
            playerDetected = true;
            hasRecentlyShot = true;
            StartCoroutine(ShootArrow(targetPlayer));
        }
        // 2. If already detected, chase if still in chase range
        else if (playerDetected && distanceToPlayer <= chaseRange && !isShooting)
        {
            agent.isStopped = false;
            agent.SetDestination(lockedTarget.position);
            animator.SetBool("isChasing", true);
        }
        // 3. Player too far, forget
        else if (distanceToPlayer > chaseRange)
        {
            animator.SetBool("isChasing", false);
            targetPlayer = null;
            playerDetected = false;
            wayPointFound = false;
        }
        else if (targetPlayer == null)
        {
            return;
        }
    }

    Transform findATarget()
    {
        if (wayPointFound) return lockedTarget;

        int randomNum = Random.Range(1, 5);
        switch (randomNum)
        {
            case 1:
                lockedTarget = bottomPoint;
                break;
            case 2:
                lockedTarget = leftPoint;
                break;
            case 3:
                lockedTarget = rightPoint;
                break;
            case 4:
                lockedTarget = topPoint;
                break;
        }

        // Debug.Log("Locked target: " + lockedTarget.name);
        wayPointFound = true;
        return lockedTarget;
    }

    IEnumerator ShootArrow(Transform target)
    {
        isShooting = true;
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.5f); 

        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();        
        Vector3 archerPos = transform.position;
        arrowScript.Shoot(target.position, archerPos);

        yield return new WaitForSeconds(shootingCooldown);
        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(0.5f);
        isShooting = false;
        hasRecentlyShot = false;
    }
}
