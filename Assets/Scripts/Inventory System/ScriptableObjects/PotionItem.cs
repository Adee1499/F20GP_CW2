using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion_", menuName = "Inventory System/Items/Potion Item")]
public class PotionItem : InventoryItem
{
    public void Awake()
    {
        itemType = ItemType.Potion;
    }
}
