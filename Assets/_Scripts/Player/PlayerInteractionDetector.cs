using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange;

    public void OnInteract()
    {
        if (interactableInRange != null)
        {
            interactableInRange.Interact();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            interactableInRange = interactable;
        }
    }   
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
        }
    }
}
