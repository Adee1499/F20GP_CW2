using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IDropHandler
{
    public EquipmentSlot equipmentSlot;
    [SerializeField] Transform _itemParent;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) {
            GameObject droppedItem = InventoryUI.Instance.CurrentItem;
            InventoryUI.Instance.CurrentItem = null;

            if (droppedItem != null) {
                DraggableItemUI droppedItemSO = droppedItem.GetComponent<DraggableItemUI>();

                // If the item was drop on the inventory slots
                if (equipmentSlot == EquipmentSlot.None) {
                    // Unequip this item
                    EquipmentManager.Instance.UnequipItem(droppedItemSO.item.equipmentSlot);
                    // Move over to inventory
                    droppedItem.transform.SetParent(InventoryUI.Instance.ItemsContainer);
                }

                if (equipmentSlot == droppedItemSO.item.equipmentSlot) {
                    EquipItem(droppedItem);
                }
            }
        }
    }

    public void EquipItem(GameObject item)
    {
        DraggableItemUI itemSO = item.GetComponent<DraggableItemUI>();

        if (XPSystem.Instance.GetCurrentLevel() >= (itemSO.item as EquipmentItem).levelRequired) {
            itemSO.equipped = true;

            // Check if there already is something equipped
            if (_itemParent.childCount > 0) {
                UnequipItem(_itemParent.GetChild(0).gameObject);
            }

            item.transform.SetParent(_itemParent);
            item.transform.localPosition = new Vector3(0, 0, 0);
            EquipmentManager.Instance.EquipItem(itemSO.item);

            // Update stats
            if (itemSO.item.equipmentSlot == EquipmentSlot.Weapon) {
                InventoryUI.Instance.attackStat += (itemSO.item as EquipmentItem).attackValue;
            } else if (itemSO.item.equipmentSlot != EquipmentSlot.None) {
                InventoryUI.Instance.defenceStat += (itemSO.item as EquipmentItem).defenseValue;
            }
        }
    }

    public void UnequipItem(GameObject item)
    {
        DraggableItemUI itemSO = item.GetComponent<DraggableItemUI>();
        itemSO.equipped = false;
        // Unequip this item
        EquipmentManager.Instance.UnequipItem(equipmentSlot);
        // Move over to inventory
        item.transform.SetParent(InventoryUI.Instance.ItemsContainer);

        // Update stats
        if (itemSO.item.equipmentSlot == EquipmentSlot.Weapon) {
            InventoryUI.Instance.attackStat -= (itemSO.item as EquipmentItem).attackValue;
        } else if (itemSO.item.equipmentSlot != EquipmentSlot.None) {
            InventoryUI.Instance.defenceStat -= (itemSO.item as EquipmentItem).defenseValue;
        }
    }
}
