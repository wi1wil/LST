using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using System.Collections;

public class PlayerMovementScript : Entity, IDamageable
{
    public float speed = 5f;
    public float jumpForce = 1.5f;
    public float jumpTimer = 0f;
    public float jumpDuration = 0.5f;

    private int maxHealth = 100;
    public int currentHealth { get; set; }
    public Material flashMaterial;
    public Material originalMaterial;

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
    IcicleSurgeScript icicleSurge;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
        abilitiesScript = GetComponent<DashScript>();
        icicleSurge = GetComponent<IcicleSurgeScript>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && icicleSurge.isSurging)
        {
            return;
        }
        else if (context.performed && icicleSurge.isCharging)
        {
            // Debug.Log("Movement activated, cancling charge");
            icicleSurge.CancleCharge();
        }

        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && icicleSurge.isCharging)
        {
            // Debug.Log("Jumping activated, cancling charge");
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
        if (abilitiesScript.isDashing || icicleSurge.isSurging)
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
