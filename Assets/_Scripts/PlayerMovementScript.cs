using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using System;

public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 1.5f;
    public float jumpTimer = 0f;
    public float jumpDuration = 0.5f;
    public float jumpHeight = 0f;
    public float maxJump = 2;
    public float currentJump = 0;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public Vector2 movementInput;
    private bool isJumping = false;
    private bool isGrounded = true;
    private Vector2 basePosition;

    DashScript abilitiesScript;
    IcicleSurge icicleSurge;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        abilitiesScript = GetComponent<DashScript>();
        icicleSurge = GetComponent<IcicleSurge>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && icicleSurge.isSurging)
        {
            return;
        }
        else if (context.performed && icicleSurge.isCharging)
        {
            Debug.Log("Movement activated, cancling charge");
            icicleSurge.CancleCharge();
        }

        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed && icicleSurge.isCharging)
        {
            Debug.Log("Jumping activated, cancling charge");
            icicleSurge.CancleCharge();
        }

        if (context.performed && icicleSurge.isSurging)
        {
            return;
        }

        if (context.performed && currentJump < maxJump)
        {
            currentJump++;
            isJumping = true;
            jumpTimer = 0f;
            basePosition = transform.localPosition;
        }
    }

    void Update()
    {
        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float progress = jumpTimer / jumpDuration;

            jumpHeight = Mathf.Sin(progress * Mathf.PI) * jumpForce;
            transform.localPosition = new Vector2(basePosition.x, basePosition.y + jumpHeight);

            if (progress >= 1f)
            {
                isJumping = false;
                if (currentJump >= maxJump)
                {
                    isGrounded = false;
                }
                else
                {
                    isGrounded = true;
                }
            }
        }

        if (!isJumping && !isGrounded)
        {
            isGrounded = true;
            currentJump = 0;
            transform.localPosition = new Vector2(basePosition.x, basePosition.y);
            basePosition = transform.localPosition;
        }
    }

    void FixedUpdate()
    {
        if(abilitiesScript.isDashing || icicleSurge.isSurging)
        {
            return; 
        }

        Vector2 movementDir = movementInput.normalized;
        rb.velocity = movementDir * speed;
        if (movementDir.sqrMagnitude > 0.01f)
        {
            anim.SetBool("isRunning", true);
            if (movementDir.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (movementDir.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }
}
