using UnityEngine;
using System.Collections;
using UnityEditor.MPE;

public class ArrowScript : MonoBehaviour
{
    public float speed;
    public float relativeHeight;
    public float maxSpeed;
    public float distance;
    public bool atTarget = false;
    public int damage = 2;
    Vector3 target;

    private Vector3 arrowStartPos;
    private Vector3 arrowEndPos;
    private Vector3 arrowRange;

    private Vector3 arrowMoveDirection;

    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] AnimationCurve correctionCurve;
    [SerializeField] AnimationCurve speedCurve;

    ArrowVisualScript arrowVisualScript;

    private float nextArrowPosY;
    private float nextArrowPosX;
    private float absCorrectionY;
    private float absCorrectionX;
    private float distanceToTargetToDestroyArrow = 0.1f;


    void Awake()
    {
        arrowVisualScript = GetComponent<ArrowVisualScript>();
    }

    void Update()
    {
        UpdateArrowPos();

        if (Vector3.Distance(transform.position, arrowEndPos) < distanceToTargetToDestroyArrow)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 toTarget = arrowEndPos - arrowStartPos;
        Vector3 toArrow = transform.position - arrowStartPos;
        if (Vector3.Dot(toArrow, toTarget) / toTarget.sqrMagnitude > 1f)
        {
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector3 target, Vector3 archerPos)
    {
        this.target = target;
        arrowStartPos = archerPos;
        arrowStartPos.z = 0f;
        arrowEndPos = target;

        distance = target.x - arrowStartPos.x;
        this.relativeHeight = Mathf.Abs(distance) * relativeHeight;

        arrowRange = arrowEndPos - arrowStartPos;
        arrowVisualScript.Initialize(arrowRange, arrowStartPos, target);
    }

    void UpdateArrowPos()
    {
        if (Mathf.Abs(arrowRange.normalized.x) < Mathf.Abs(arrowRange.normalized.y))
        {
            // If the arrow is moving more vertically than horizontally
            if (arrowRange.y < 0)
                speed = -speed;

            UpdatePosWithXCurve();
        }
        else
        {
            // If the arrow is moving more horizontally than vertically
            if (arrowRange.x < 0)
                speed = -speed;

            UpdatePosWithYCurve();
        }
    }

    void UpdatePosWithXCurve()
    {
        float nextPosY = transform.position.y + speed * Time.deltaTime;
        float progress = (nextPosY - arrowStartPos.y) / arrowRange.y;

        float curveX = animationCurve.Evaluate(progress);
        nextArrowPosX = curveX * relativeHeight;

        float correctionX = correctionCurve.Evaluate(progress);
        absCorrectionX = correctionX * arrowRange.x;

        if (arrowRange.x > 0 && arrowRange.y > 0)
            nextArrowPosX = -nextArrowPosX;

        if (arrowRange.x < 0 && arrowRange.y < 0)
            nextArrowPosX = -nextArrowPosX;

        float nextPosX = arrowStartPos.x + nextArrowPosX + absCorrectionX;

        CalcNextSpeed(progress);
        Vector3 newPos = new Vector3(nextPosX, nextPosY, 0f);
        arrowMoveDirection = newPos - transform.position;

        transform.position = newPos;
    }

    void UpdatePosWithYCurve()
    {
        float nextPosX = transform.position.x + speed * Time.deltaTime;
        float progress = (nextPosX - arrowStartPos.x) / arrowRange.x;

        float curveY = animationCurve.Evaluate(progress);
        nextArrowPosY = curveY * relativeHeight;

        float correctionY = correctionCurve.Evaluate(progress);
        absCorrectionY = correctionY * arrowRange.y;

        float nextPosY = arrowStartPos.y + nextArrowPosY + absCorrectionY;

        CalcNextSpeed(progress);

        Vector3 newPos = new Vector3(nextPosX, nextPosY, 0f);
        arrowMoveDirection = newPos - transform.position;

        transform.position = newPos;
    }

    void CalcNextSpeed(float progress)
    {
        float nextMoveSpeed = speedCurve.Evaluate(progress);
        speed = nextMoveSpeed * maxSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && collision.CompareTag("Player"))
        {
            Debug.Log("Arrow hit player: " + collision.name);
            damageable.TakeDamage(damage, transform.position);
            Destroy(gameObject);
        }
    }

    public Vector3 GetArrowMoveDirection()
    {
        return arrowMoveDirection;
    }

    public float AbsCorrectionY()
    {
        return absCorrectionY;
    }

    public float GetNextPosY()
    {
        return nextArrowPosY;
    }
    
    public float AbsCorrectionX()
    {
        return absCorrectionX;
    }

    public float GetNextPosX()
    {
        return nextArrowPosX;
    }
}