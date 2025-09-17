using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectileScript : MonoBehaviour
{
    public GameObject fireballParticlePrefab;
    public float speed = 10f;
    public float lifetime = 3f;
    public int damage;
    
    private Vector2 direction;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Env"))
        {
            Destroy(gameObject);
            return;
        }

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && !collision.CompareTag("Player"))
        {
            Vector2 enemyPos = collision.transform.position;
            Destroy(Instantiate(fireballParticlePrefab, enemyPos, Quaternion.identity), 2f);
            damageable.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }
}
