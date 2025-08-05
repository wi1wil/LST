using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public int damage = 2; // Adjust the damage value as needed
    Vector3 target;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Shoot()
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.velocity = direction * speed;

        if (target.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (target.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && collision.CompareTag("Player"))
        {
            damageable.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Environment"))
        {
            // Give warning to Archer to reposition if the arrow hits the environment more than once
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 3f);
        }
    }

    public void Initialize(Vector3 target)
    {
        this.target = target;
    }
}
