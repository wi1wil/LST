using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Threading;


public class ArcherScript : Entity, IDamageable
{
    public float speed;
    public float knockbackForce = 5f;

    public float shootingCooldown = 1f;
    private float roamingCooldown = 10f;
    private float roamTimer = 10f;
    
    public bool isChasing = false;
    public bool isShooting = false;
    public bool isRoaming = false;
    public bool hasRecentlyShot = false;
    public bool playerDetected = false;
    public bool wayPointFound = false;

    public float chaseRange = 15f;
    public float detectionRange = 7.5f;
    public float avoidanceRange = 2.5f;
    public float avoidanceForce = 2f;

    private float xScale;
    public Transform topPoint;
    public Transform bottomPoint;
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform lockedTarget;
    public Vector2 spawnLocation;
    public GameObject arrowPrefab;
    public Transform healthBar;
    public LayerMask obstacleLayer;
    public Material flashMaterial;
    public Transform targetPlayer = null;

    Animator animator;
    NavMeshAgent agent;
    SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private int maxHealth = 50;
    public int currentHealth { get; set; }

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
        currentHealth = maxHealth;
        xScale = healthBar.localScale.x;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        spawnLocation = transform.position;
    }
    
    void Update()
    {
        DetectPlayer();
        UpdateBehavior();
        

        if (!playerDetected && !isShooting && targetPlayer == null)
        {
            roamTimer -= Time.deltaTime;
            // Debug.Log("Roam timer: " + roamTimer);
            if (roamTimer < 0f && !isRoaming)
            {
                StartCoroutine(InitializeRoaming());
            }
        }
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

    public void TakeDamage(int damage, Vector3 hitSource)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Handle enemy death
            Debug.Log("Archer has died.");
            Destroy(gameObject);
        }
        else
        {
            Vector2 knockbackDirection = (transform.position - hitSource).normalized;

            StartCoroutine(DamageFlash());
            StartCoroutine(TakeKnockback(knockbackDirection));
            UpdateHealth();
        }
    }

    public void UpdateHealth()
    {
        float ratio = Mathf.Clamp01(currentHealth / (float)maxHealth);
        healthBar.localScale = new Vector3(xScale * ratio, healthBar.localScale.y, healthBar.localScale.z);
        Debug.Log("Health updated: " + currentHealth + "/" + maxHealth);
    }

    IEnumerator DamageFlash()
    {
        Material originalMaterial = spriteRenderer.material;
        spriteRenderer.material = flashMaterial; // Use the flash material
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = originalMaterial; // Restore the original material
    }

    IEnumerator TakeKnockback(Vector2 knockbackDirection)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

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
            FlipSprite();
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

        Debug.Log("Locked target: " + lockedTarget.name);
        wayPointFound = true;
        return lockedTarget;
    }

    IEnumerator ShootArrow(Transform target)
    {
        isShooting = true;
        FlipSprite();
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.5f); // Wait for the shooting animation to finish

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

    IEnumerator InitializeRoaming()
    {
        isRoaming = true;
        Vector2 randomDirection = Random.insideUnitCircle.normalized * Random.Range(1f, detectionRange);
        Vector2 newDestination = spawnLocation + randomDirection;

        NavMeshHit hit;
        // Check if the new destination is valid on the NavMesh
        if (NavMesh.SamplePosition(newDestination, out hit, 1f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            animator.SetBool("isChasing", true);
            agent.SetDestination(hit.position);

            while (Vector2.Distance(transform.position, hit.position) > 0.1f)
            {
                FlipSprite();
                yield return null;
            }

            agent.isStopped = true;
            animator.SetBool("isChasing", false);
        }
        // Failed to find a valid position, stop roaming
        else
        {
            // Debug.LogWarning("Failed to find a valid position for roaming");
            InitializeRoaming();
        }

        isRoaming = false;
        roamTimer = roamingCooldown; // Reset the roam timer
        yield return new WaitForSeconds(roamingCooldown);
    }

    void FlipSprite()
    {
        if (isShooting)
        {
            if (targetPlayer == null) return;
            if (targetPlayer.position.x > transform.position.x)
            {
                spriteRenderer.flipX = false;
            }
            else if (targetPlayer.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            if (agent.velocity.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (agent.velocity.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}
