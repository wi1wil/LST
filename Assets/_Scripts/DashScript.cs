using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using JetBrains.Annotations;
using System;

public class DashScript : MonoBehaviour
{
    public float dashSpeed;
    public float cooldownTime = 10f;
    public bool canDash = true;
    public bool isDashing;

    public int maxDash = 2;
    public int currentDash = 0;
    public Image dashCooldownDisplay;
    public TMP_Text hotkeyText;
    public Rigidbody2D rb;

    public SpriteRenderer spriteRenderer;

    PlayerMovementScript playerMovementScript;
    IcicleSurge icicleSurge;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovementScript = GetComponent<PlayerMovementScript>();
        icicleSurge = GetComponent<IcicleSurge>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && icicleSurge.isSurging)
        {
            return;
        }
        else if (context.performed && icicleSurge.isCharging)
        {
            Debug.Log("Dash activated, cancling charge");
            icicleSurge.CancleCharge();
        }

        
        if (context.performed && canDash)
        {
            StartCoroutine(StartDash());
            hotkeyText.text = " ";
            Debug.Log("Dash activated");
        }
        else if (context.performed && !canDash)
        {
            Debug.Log("Dash is on cooldown, please wait.");
        }
    }

    IEnumerator StartDash()
    {
        currentDash++;

        Vector2 dashDirection;
        if (playerMovementScript.movementInput == Vector2.zero)
        {
            dashDirection = !spriteRenderer.flipX ? Vector2.right : Vector2.left;
        }
        else
        {
            dashDirection = playerMovementScript.movementInput.normalized;
        }

        Vector2 dashVelocity = dashDirection * dashSpeed;

        switch (currentDash)
        {
            case 1:
                isDashing = true;
                rb.velocity = dashVelocity;
                yield return new WaitForSeconds(0.2f);
                isDashing = false;
                break;
            case 2:
                canDash = false;
                isDashing = true;
                rb.velocity = dashVelocity;
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(StartCooldown());
                isDashing = false;
                break;
        }
    }

    IEnumerator StartCooldown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            dashCooldownDisplay.fillAmount = elapsedTime / cooldownTime;
            yield return null;
        }
        canDash = true;
        currentDash = 0;
        Debug.Log("Dash is ready again.");
    }
}
