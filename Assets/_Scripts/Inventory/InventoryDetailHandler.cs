using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDetailHandler : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject itemImageGO;
    [SerializeField] private GameObject itemNameGO;
    [SerializeField] private GameObject itemDescGO;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDesc;

    public static InventoryDetailHandler instance;

    void Awake()
    {
        instance = this;
    }

    public void UpdateUI(ItemsSO item)
    {
        if (item == null)
        {
            ClearUI();
            return;
        }

        itemImageGO.SetActive(true);
        itemNameGO.SetActive(true);
        itemDescGO.SetActive(true);
        itemImage.sprite = item.itemIcon;
        itemName.text = item.itemName;
        itemDesc.text = item.itemDesc;
    }

    public void ClearUI()
    {
        itemImage.sprite = null;
        itemName.text = "";
        itemDesc.text = "";
        
        itemImageGO.SetActive(false);
        itemNameGO.SetActive(false);
        itemDescGO.SetActive(false);
    }
}