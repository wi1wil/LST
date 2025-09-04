using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIScript : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryPanel != null)
        {
            if (inventoryPanel.activeSelf)
            {
                inventoryPanel.SetActive(false);
            }
            else
            {
                inventoryPanel.SetActive(true);
            }
        }
    }
}
