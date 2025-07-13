using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Animations;
using System.Numerics;
using System.Collections;

public class IcicleSurge : MonoBehaviour
{
    public Rigidbody2D rb;
    public CapsuleCollider2D player;
    public GameObject iciclesCollider;

    public bool canAttack = true;
    public float cooldownTime = 20f;
    public Image surgeCooldownDisplay;

    public bool isCharging = false;
    public float chargeTime = 0.75f;

    public bool isSurging = false;
    public float surgeTime = 1.5f;

    public Animator anim;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnSurge(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            Debug.Log("Surge activated");
            StartCoroutine(StartCharge());
        }
    }

    IEnumerator StartCharge()
    {
        Debug.Log("Charging surge");
        canAttack = false;
        isCharging = true;
        yield return new WaitForSeconds(chargeTime);
        StartCoroutine(StartSurge());
    }

    public void CancleCharge()
    {
        Debug.Log("Charge cancelled");
        StopAllCoroutines();
        isCharging = false;
        isSurging = false;
        canAttack = true;
    }

    IEnumerator StartSurge()
    {
        Debug.Log("Surge Started");
        isCharging = false;
        isSurging = true;

        bool facingRight = !spriteRenderer.flipX;
        if (facingRight)
        {
            // Offset the player's collider to the right
            Debug.Log("Facing Right, offsetting collider to the right");
            player.offset = new UnityEngine.Vector2(player.offset.x - 2f, player.offset.y);
        }
        else
        {
            // Offset the player's collider to the left
            Debug.Log("Facing Left, offsetting collider to the left");
            player.offset = new UnityEngine.Vector2(player.offset.x + 2f, player.offset.y);
        }

        // Apply to Icicle Prefab
        float offsetX = facingRight ? 1 : -1;
        UnityEngine.Vector2 spawnPos = new UnityEngine.Vector2(transform.position.x + offsetX, transform.position.y);

        yield return new WaitForSeconds(0.6f);
        StartCoroutine(SpawnIcicles(spawnPos));

        yield return new WaitForSeconds(surgeTime);
        StartCoroutine(StopSurge());
    }

    IEnumerator StopSurge()
    {
        Debug.Log("Surge ended");
        isSurging = false;
        canAttack = true;
        player.offset = UnityEngine.Vector2.zero;
        yield return null;
    }


    IEnumerator SpawnIcicles(UnityEngine.Vector2 spawnPos)
    {
        Debug.Log("Spawning Icicles");
        GameObject icicles = Instantiate(iciclesCollider, spawnPos, UnityEngine.Quaternion.identity);
        yield return new WaitForSeconds(0.7f);
        Destroy(icicles);
        Debug.Log("Icicles destroyed");
    }

    IEnumerator StartCooldown()
    {
        // float elapsedTime = 0f;
        yield return null;
    }

    void Update() {
        if (isCharging)
        {
            anim.SetBool("isCharging", true); 
        }
        else
        {
            anim.SetBool("isCharging", false);
        }

        if(isSurging)
        {
            anim.SetBool("isSurging", true);
        }
        else
        {
            anim.SetBool("isSurging", false);
        }
    }
}