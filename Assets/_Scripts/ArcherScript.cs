using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using System.Collections;
using System.ComponentModel.Design;
using NavMeshPlus.Extensions;
using UnityEngine.AI;


public class ArcherScript : Entity, IDamageable
{
    public float speed = 5f;
    public float detectionRange = 7.5f;

    public Transform topPoint;
    public Transform targetPlayer = null;

    public bool isChasing = false;
    public bool isShooting = false;
    public bool playerDetected = false;
    public float shootingCooldown = 1f;
    public float chaseRange = 15f;
    public GameObject arrowPrefab;

    public Transform healthBar;
    private float xScale;

    Animator animator;
    NavMeshAgent agent;
    SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    public int currentHealth { get; set; }
    private int maxHealth = 50;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Handle enemy death
            Debug.Log("Archer has died.");
            Destroy(gameObject);
        }
        UpdateHealth();
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
    }

    void Update()
    {
        DetectPlayer();
        UpdateBehavior();
        FlipSprite();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    public void UpdateHealth()
    {
        float ratio = Mathf.Clamp01(currentHealth / (float)maxHealth);
        healthBar.localScale = new Vector3(xScale * ratio, healthBar.localScale.y, healthBar.localScale.z);
        Debug.Log("Health updated: " + currentHealth + "/" + maxHealth);
    }

    public void DetectPlayer()
    {
        // Add in Line of Sight logic
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, chaseRange, LayerMask.GetMask("Player"));
        foreach (Collider2D hit in hits)
        {
            PlayerMovementScript player = hit.GetComponent<PlayerMovementScript>();
            if (player != null && !isShooting)
            {
                // Player detected within range
                targetPlayer = player.transform;
                Debug.Log("Player detected: " + player.gameObject.name);
            }
        }
    }

    void UpdateBehavior()
    {
        Debug.Log("Player detected: " + playerDetected);
        if (targetPlayer == null)
        {
            animator.SetBool("isChasing", false);
            agent.isStopped = true;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, topPoint.position);
        // 1. Stop and shoot if close enough
        if (distanceToPlayer < detectionRange && !isShooting)
        {
            animator.SetBool("isChasing", false);
            agent.isStopped = true;
            playerDetected = true;
            Debug.Log("Player detected" + playerDetected);
            StartCoroutine(ShootArrow());
        }
        // 2. If already detected, chase if still in chase range
        else if (playerDetected && distanceToPlayer <= chaseRange && !isShooting)
        {
            Debug.Log("Chasing player: " + targetPlayer.name);
            agent.isStopped = false;
            agent.SetDestination(topPoint.position);
            animator.SetBool("isChasing", true);
            // Avoid other archers
            // Add avoidance radius and logic to avoid clumping with other archers
        }
        // 3. Player too far, forget
        else
        {
            animator.SetBool("isChasing", false);
            agent.isStopped = true;
            targetPlayer = null;
            if (distanceToPlayer > chaseRange)
                playerDetected = false;
            InitializeRoaming();
        }
    }

    IEnumerator ShootArrow()
    {
        isShooting = true;
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.5f); // Wait for the shooting animation to finish
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(shootingCooldown);
        Debug.Log("Arrow shot!");
        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(0.5f); 
        isShooting = false;
    }

    void InitializeRoaming()
    {
        // Start walking around its tower, and not wander too far
    }

    void FlipSprite()
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
