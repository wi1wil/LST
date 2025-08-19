using UnityEngine;

public class OnDragExample : MonoBehaviour
{
    public SpriteRenderer spRend;
    public Material originalMaterial;
    public Material flashMaterial;

    void Start()
    {
        spRend = GetComponent<SpriteRenderer>();
        originalMaterial = spRend.material;
    }

    void OnMouseDrag()
    {
        Debug.Log("Im dragging this shit!");
        spRend.material = flashMaterial;
    }

    void OnMouseUp()
    {
        spRend.material = originalMaterial;
        Debug.Log("I dropped it!");
    }
}
