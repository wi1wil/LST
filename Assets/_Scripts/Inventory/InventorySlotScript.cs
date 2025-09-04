using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotScript : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemQty;
    [SerializeField] private Button button;

    private ItemsSO currentItem;

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    public void UpdateUI(ItemsSO item, int quantity)
    {
        if (item != null && quantity > 0)
        {
            currentItem = item;
            itemIcon.sprite = item.itemIcon;
            itemIcon.enabled = true;
            itemQty.text = quantity > 1 ? quantity.ToString() : "";
            gameObject.SetActive(true);
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemQty.text = "";
        gameObject.SetActive(false);
    }

    private void OnClick()
    {
        if (currentItem != null)
            InventoryDetailHandler.instance.UpdateUI(currentItem);
    }
}
