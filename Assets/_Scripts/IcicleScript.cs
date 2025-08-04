using System.Security.Cryptography;
using UnityEngine;

public class IcicleScript : MonoBehaviour
{
    public GameObject icicleParticlePrefab;
    public int damage;

    void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null || !collision.CompareTag("Player"))
        {
            Debug.Log("Icicle hit: " + collision.gameObject.name + " with damage: " + damage);
            Vector2 enemyPos = collision.transform.position;
            Destroy(Instantiate(icicleParticlePrefab, enemyPos, Quaternion.identity), 2f);
            Debug.Log("Icicle hit enemy at position: " + enemyPos);
            damageable.TakeDamage(damage, transform.position); // Adjust damage value as needed
        }
    }
}
