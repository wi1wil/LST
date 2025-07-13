using UnityEngine;

public class IcicleScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Icicle collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 enemyPos = collision.transform.position;
            Debug.Log("Icicle hit enemy at position: " + enemyPos);
        }
    }
}
