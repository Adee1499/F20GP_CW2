using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment_", menuName = "Inventory System/Items/Equipment Item")]
public class EquipmentItem : InventoryItem
{
    public void Awake()
    {
        itemType = ItemType.Equipment;
    }
}
