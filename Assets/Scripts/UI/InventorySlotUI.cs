using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) {
            InventoryItem droppedItem = InventoryUI.Instance.CurrentItem;
            InventoryUI.Instance.CurrentItem = null;

            if (droppedItem != null) {
                transform.Find("ItemSilhouette").gameObject.SetActive(false);
                GameObject equippedItem = Instantiate(InventoryUI.Instance.EquipmentItemPrefab, transform);
                equippedItem.transform.Find("ItemImage").GetComponent<Image>().sprite = droppedItem.inventoryIcon;
                equippedItem.GetComponent<DraggableItemUI>().item = droppedItem;
            }
        }
    }
}
