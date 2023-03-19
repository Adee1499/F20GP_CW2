using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Equipable Item")]
public class EquipableItem : InventoryItem
{
    public enum EquipmentSlot {
        Weapon,
        Head,
        Body,
        Legs,
        Boots,
        Arms,
        Accessory
    }

    public EquipmentSlot equipmentSlot;
    public GameObject mesh;
}
