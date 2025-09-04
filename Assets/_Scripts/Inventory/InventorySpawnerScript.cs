using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySpawnerScript : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private int poolSize = 35;
    private List<InventorySlotScript> slots = new List<InventorySlotScript>();

    private void Awake()
    {
        SpawnInventory();
    }

    public void SpawnInventory()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, container);
            slot.SetActive(false);
            slots.Add(slot.GetComponent<InventorySlotScript>());
        }
    }

    public void UpdateInventory(List<ItemStack> items)
    {
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }

        for (int i = 0; i < items.Count && i < slots.Count; i++)
        {
            slots[i].UpdateUI(items[i].item, items[i].quantity);
        }
    }
}
