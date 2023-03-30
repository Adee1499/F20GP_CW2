using UnityEngine;

public enum ItemType {
    Potion,
    Equipment
}

public abstract class InventoryItem : ScriptableObject
{
    public ItemType itemType;
    public Sprite inventoryIcon;
    public GameObject onPlayerPrefab;
    public EquipmentSlot equipmentSlot;
    public GameObject onGroundPrefab;
    public int sellValue = 50;
    public string itemName;
    [TextArea(3,5)]
    public string description;
}
