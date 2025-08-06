using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    public float relativeHeight;
    public int damage = 2;
    Vector3 target;

    private Vector3 arrowStartPos;
    private Vector3 arrowEndPos;
    private Vector3 arrowRange;

    SpriteRenderer spriteRenderer;
    [SerializeField] AnimationCurve animationCurve;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateArrowPos();
    }

    public void Shoot(Vector3 target, Vector3 archerPos)
    {
        this.target = target;
        arrowStartPos = archerPos;
        arrowStartPos.z = 0f;
        arrowEndPos = target;

        float distance = target.x - arrowStartPos.x;
        this.relativeHeight = Mathf.Abs(distance) * relativeHeight;

        arrowRange = arrowEndPos - arrowStartPos;

        FlipSprite();
    }

    void UpdateArrowPos()
    {
        float nextPosX = transform.position.x + speed * Time.deltaTime;
        float progress = (nextPosX - arrowStartPos.x) / arrowRange.x;
        float curveY = animationCurve.Evaluate(progress);
        float nextPosY = arrowStartPos.y + curveY * relativeHeight;

        Vector3 newPos = new Vector3(nextPosX, nextPosY, 0f);
        transform.position = newPos;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && collision.CompareTag("Player"))
        {
            damageable.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Environment"))
        {
            // Give warning to Archer to reposition if the arrow hits the environment more than once
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 3f);
        }
    }

    void FlipSprite()
    {
        if (target.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (target.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }
}
