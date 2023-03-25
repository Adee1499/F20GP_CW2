using UnityEngine;
using UnityEngine.EventSystems;

public class MerchantSellUI : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) {
            GameObject droppedItem = InventoryUI.Instance.CurrentItem;
            InventoryUI.Instance.CurrentItem = null;

            if (droppedItem != null) {
                DraggableItemUI droppedItemSO = droppedItem.GetComponent<DraggableItemUI>();

                // If merchant has enough gold to buy the item
                if (Merchant.Instance.inventory.gold >= droppedItemSO.item.sellValue) {
                    // Add item to merchant's inventory
                    Merchant.Instance.BuyItemFromPlayer(droppedItemSO.item);

                    // Remove item from player's inventory
                    InventoryUI.Instance.RemoveInventoryItem(droppedItemSO.item);
                }
            }
        }
    }
}
