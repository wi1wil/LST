using System.Collections.Generic;
using UnityEngine;

public class InventoryManagerScript : MonoBehaviour
{
    public static InventoryManagerScript instance;

    [Header("Available Items")]
    public List<ItemsSO> availableItems; 

    private InventorySpawnerScript inventorySpawner;
    public List<ItemStack> items = new List<ItemStack>();

    private void Awake()
    {
        instance = this;
        inventorySpawner = GetComponent<InventorySpawnerScript>();
    }

    public ItemsSO GetItemByID(string id)
    {
        return availableItems.Find(i => i.itemID == id);
    }

    public void AddItem(string itemID, int amount)
    {
        ItemsSO newItem = GetItemByID(itemID);
        if (newItem == null)
        {
            Debug.LogWarning($"Item with ID {itemID} not found!");
            return;
        }

        ItemStack stack = items.Find(i => i.item == newItem);
        if (stack != null)
        {
            stack.quantity += amount;
        }
        else
        {
            items.Add(new ItemStack(newItem, amount));
        }

        inventorySpawner.UpdateInventory(items);
    }

    public void RemoveItem(string itemID, int amount)
    {
        ItemsSO item = GetItemByID(itemID);
        if (item == null)
        {
            Debug.LogWarning($"Item with ID {itemID} not found!");
            return;
        }

        ItemStack stack = items.Find(i => i.item == item);
        if (stack != null)
        {
            stack.quantity -= amount;
            if (stack.quantity <= 0)
                items.Remove(stack);
        }

        inventorySpawner.UpdateInventory(items);
    }

    public SaveData ToSaveData()
    {
        SaveData saveData = new SaveData();
        foreach (var stack in items)
        {
            saveData.items.Add(new InventoryItemData
            {
                itemID = stack.item.itemID,
                quantity = stack.quantity
            });
        }
        return saveData;
    }

    public void FromSaveData(SaveData saveData)
    {
        items.Clear();
        foreach (var saved in saveData.items)
        {
            ItemsSO itemSO = GetItemByID(saved.itemID);
            if (itemSO != null)
                items.Add(new ItemStack(itemSO, saved.quantity));
            else
                Debug.LogWarning($"Item ID {saved.itemID} not found in availableItems!");
        }
        inventorySpawner.UpdateInventory(items);
    }
}
