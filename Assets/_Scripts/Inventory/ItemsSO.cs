using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ItemsSO")]

public class ItemsSO : ScriptableObject
{
    public string itemID;
    public string itemName;
    public string itemDesc;
    public Sprite itemIcon;
}

[System.Serializable]
public class ItemStack
{
    public ItemsSO item;
    public int quantity;

    public ItemStack(ItemsSO item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}