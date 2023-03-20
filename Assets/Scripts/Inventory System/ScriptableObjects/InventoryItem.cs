using UnityEngine;

public enum ItemType {
    Potion,
    Equipment
}

public abstract class InventoryItem : ScriptableObject
{

    public ItemType itemType;
    public Sprite inventoryIcon;
    public GameObject prefab;
}