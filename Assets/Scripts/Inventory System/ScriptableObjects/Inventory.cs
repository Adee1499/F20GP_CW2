using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventorySlot> items = new List<InventorySlot>();
    public int gold;

    public void AddItem(InventoryItem item, int amount)
    {
        bool hasItem = false;
        foreach(InventorySlot itemSlot in items) {
            if(itemSlot.item == item) {
                itemSlot.AddAmount(amount);
                hasItem = true;
                break;
            }
        }

        if (!hasItem) {
            items.Add(new InventorySlot(item, amount));
        }
    }
}

[System.Serializable]
public class InventorySlot 
{
    public InventoryItem item;
    public int amount;
    public InventorySlot(InventoryItem _item, int _amount) 
    {
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value) { amount += value; }

    public bool CompareItem(InventoryItem _item)
    {
        if (_item == item) {
            return true;
        } else {
            return false;
        }
    }
}