using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ItemsSO")]

public class ItemsSO : ScriptableObject
{
    public string itemID;
    public string itemName;
    public string itemDesc;
    public Sprite itemSprite;
}

[System.Serializable]
public class itemQuantity
{

}