using UnityEngine;

public class ArcherRangeTrigger : MonoBehaviour
{
    public string rangeType;
    private ArcherStateManager archer;

    void Start()
    {
        archer = GetComponentInParent<ArcherStateManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (archer.LineOfSight(other.transform))
        {
            archer.OnRangeTriggerEnter(rangeType, other);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (archer.LineOfSight(other.transform))
        {
            archer.OnRangeTriggerEnter(rangeType, other);
        }
        else
        {
            archer.OnRangeTriggerExit(rangeType, other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        archer.OnRangeTriggerExit(rangeType, other);
    }
}