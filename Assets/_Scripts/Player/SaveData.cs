using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<InventoryItemData> items = new List<InventoryItemData>();
}

[System.Serializable]
public class InventoryItemData
{
    public string itemID;
    public int quantity;
}