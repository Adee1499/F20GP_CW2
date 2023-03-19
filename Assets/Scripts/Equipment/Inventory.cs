using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();
    public int maxCapacity = 10;
    public Dictionary<EquipableItem.EquipmentSlot, EquipableItem> equippedItems = new Dictionary<EquipableItem.EquipmentSlot, EquipableItem>();

    public void EquipItem(EquipableItem item)
    {
        if (item == null || items.Contains(item) == false) return;

        equippedItems[item.equipmentSlot] = item;
        items.Remove(item);
    }

    public void UnequipItem(EquipableItem.EquipmentSlot slot)
    {
        if (equippedItems.ContainsKey(slot) == false) return;
        
        items.Add(equippedItems[slot]);
        equippedItems.Remove(slot);
    }
}
