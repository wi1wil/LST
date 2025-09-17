using System.Collections;
using UnityEngine;

public class ArcherHealthScript : MonoBehaviour, IDamageable, Entity
{
    public float knockbackForce = 5f;
    private int maxHealth = 50;
    public int currentHealth { get; set; }

    private float xScale;
    public Transform healthBar;
    public Material flashMaterial;
    
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        xScale = healthBar.localScale.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }


    public void TakeDamage(int damage, Vector3 hitSource)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
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
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = originalMaterial;
    }

    IEnumerator TakeKnockback(Vector2 knockbackDirection)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

}
