using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemsSO addedItem;
    [SerializeField] private int addedAmount = 1;

    public void Interact()
    {
        Debug.Log($"Added item {addedItem}");
        InventoryManagerScript.instance.AddItem(addedItem.itemID, addedAmount);
        Destroy(gameObject);    
    }
}
