using System.Security.Cryptography;
using UnityEngine;

public class IcicleScript : MonoBehaviour
{
    public GameObject icicleParticlePrefab;
    public int damage;

    void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && !collision.CompareTag("Player"))
        {
            Vector2 enemyPos = collision.transform.position;
            Destroy(Instantiate(icicleParticlePrefab, enemyPos, Quaternion.identity), 2f);
            damageable.TakeDamage(damage, transform.position);
        }
    }
}
