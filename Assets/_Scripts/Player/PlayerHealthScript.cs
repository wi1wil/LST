using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : MonoBehaviour, IDamageable, Entity
{
    private int maxHealth = 100;
    public int currentHealth { get; set; }
    public Material flashMaterial;
    public Material originalMaterial;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    public void TakeDamage(int damage, Vector3 hitSource)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
            Destroy(gameObject);
        }
        else
        {
            Vector2 knockbackDirection = (transform.position - hitSource).normalized;
            StartCoroutine(DamageFlash());
            StartCoroutine(TakeKnockback(knockbackDirection));
        }
    }

    IEnumerator DamageFlash()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = originalMaterial;
    }
    
    IEnumerator TakeKnockback(Vector2 knockbackDirection)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * 2f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        rb.velocity = Vector2.zero;
    }
}

