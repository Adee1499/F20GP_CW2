using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment_", menuName = "Inventory System/Items/Equipment Item")]
public class EquipmentItem : InventoryItem
{
    public int defenseValue = 5;
    public LootRarity lootRarity;

    public void Awake()
    {
        itemType = ItemType.Equipment;
    }
}

public enum EquipmentSlot {
    None,
    Helmet,
    Body,
    Hands,
    Legs,
    Boots,
    Weapon,
    Cape
}
