using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI _instance;
    public static InventoryUI Instance { get { return _instance; }}
    [HideInInspector] public Inventory inventory;
    public Transform ItemsContainer;
    [SerializeField] private GameObject _defaultItemPrefab;
    [SerializeField] private GameObject _equipmentItemPrefab;
    public GameObject EquipmentItemPrefab { get { return _equipmentItemPrefab; }}
    [HideInInspector] public GameObject CurrentItem;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    // UI Elements references
    public GameObject UI_Inventory;
    public GameObject UI_Equipment;

    void Start()
    {
        _instance = this;
        PopulateInventory();
    }

    void Update()
    {
        RefreshInventory();
    }

    public void PopulateInventory()
    {
        for (int i = 0; i < inventory.items.Count; i++) {
            InsertInventoryItem(inventory.items[i]);
        }
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < inventory.items.Count; i++) {
            if (itemsDisplayed.ContainsKey(inventory.items[i])) {
                if (itemsDisplayed[inventory.items[i]].GetComponent<DraggableItemUI>().item.itemType != ItemType.Equipment) {
                    itemsDisplayed[inventory.items[i]].GetComponentInChildren<TextMeshProUGUI>().text = inventory.items[i].amount.ToString();
                }
            } else {
                InsertInventoryItem(inventory.items[i]);
            }
        }
    }

    private void InsertInventoryItem(InventorySlot itemSlot)
    {
        GameObject newItem = null;
        if (itemSlot.item.itemType == ItemType.Equipment) {
            newItem = Instantiate(_equipmentItemPrefab, ItemsContainer);
        } else {
            newItem = Instantiate(_defaultItemPrefab, ItemsContainer);
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = itemSlot.amount.ToString();
        }
        newItem.transform.Find("ItemImage").GetComponent<Image>().sprite = itemSlot.item.inventoryIcon;
        newItem.GetComponent<DraggableItemUI>().item = itemSlot.item;
        itemsDisplayed.Add(itemSlot, newItem);
    }

    public void RemoveInventoryItem(InventoryItem item)
    {
        foreach(InventorySlot slot in inventory.items) {
            if (slot.CompareItem(item)) {
                inventory.items.Remove(slot);
                itemsDisplayed.Remove(slot);
                break;
            }
        }
    }
}
