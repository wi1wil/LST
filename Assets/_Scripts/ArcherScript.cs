using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class ArcherScript : Entity, IDamageable 
{
    public float speed = 5f;
    public float range = 7.5f;

    public Transform topPoint;

    public bool isShooting = false;
    public float shootingCooldown = 1f;
    public float shootingRange = 15f;
    public GameObject arrowPrefab;

    public Transform healthBar;
    private float xScale;

    Animator animator;

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
        currentHealth = maxHealth;
        xScale = healthBar.localScale.x;
    }

    void Update()
    {
        DetectPlayer();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, range);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }

    public void UpdateHealth()
    {
        float ratio = Mathf.Clamp01(currentHealth / (float)maxHealth);
        healthBar.localScale = new Vector3(xScale * ratio, healthBar.localScale.y, healthBar.localScale.z);
        Debug.Log("Health updated: " + currentHealth + "/" + maxHealth);
    }

    public void DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                float topDist = Vector2.Distance(topPoint.position, transform.position);
                if (distance <= range)
                {

                }
            }
            else
            {

            }
        }
    }
}
