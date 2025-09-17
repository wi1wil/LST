using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // for Image

public class FireballScript : MonoBehaviour
{
    [Header("Fireball Settings")]
    public SpriteRenderer playerSprite; 
    public GameObject fireballPrefab;
    public Transform firePointLeft;
    public Transform firePointRight;

    [Header("Cooldown")]
    public Image fireballCooldownDisplay; 
    public float fireballCooldown = 2f;
    private bool isOnCooldown = false;

    public void OnFireball(InputAction.CallbackContext context)
    {
        if (context.performed && !isOnCooldown)
        {
            ShootFireball();
            StartCoroutine(FireballCooldownRoutine());
        }
    }

    private void ShootFireball()
    {
        Debug.Log("Fireball activated");

        bool facingRight = !playerSprite.flipX;
        Transform firePoint = facingRight ? firePointRight : firePointLeft;
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        if (!facingRight)
        {
            Vector3 scale = fireball.transform.localScale;
            scale.x *= -1;
            fireball.transform.localScale = scale;
        }

        fireball.GetComponent<FireballProjectileScript>().Launch(dir);
    }

    private IEnumerator FireballCooldownRoutine()
    {
        isOnCooldown = true;

        float timer = fireballCooldown;
        fireballCooldownDisplay.fillAmount = 0f;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            fireballCooldownDisplay.fillAmount = 1 - timer / fireballCooldown;
            yield return null;
        }

        fireballCooldownDisplay.fillAmount = 1f;
        isOnCooldown = false;
    }
}