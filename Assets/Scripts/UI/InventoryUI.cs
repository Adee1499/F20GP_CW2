using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private static InventoryUI _instance;
    public static InventoryUI Instance { get { return _instance; }}
    private Inventory _inventory;
    public Inventory Inventory { set { _inventory = value; } }
    [SerializeField] private Transform _itemsContainer;
    [SerializeField] private GameObject _defaultItemPrefab;
    [SerializeField] private GameObject _equipmentItemPrefab;
    public GameObject EquipmentItemPrefab { get { return _equipmentItemPrefab; }}
    public InventoryItem CurrentItem;

    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

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
        for (int i = 0; i < _inventory.items.Count; i++) {
            InsertInventoryItem(_inventory.items[i]);
        }
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < _inventory.items.Count; i++) {
            if (itemsDisplayed.ContainsKey(_inventory.items[i])) {
                itemsDisplayed[_inventory.items[i]].GetComponentInChildren<TextMeshProUGUI>().text = _inventory.items[i].amount.ToString();
            } else {
                InsertInventoryItem(_inventory.items[i]);
            }
        }
    }

    private void InsertInventoryItem(InventorySlot itemSlot)
    {
        GameObject newItem = null;
        if (itemSlot.item.itemType == ItemType.Equipment) {
            newItem = Instantiate(_equipmentItemPrefab, _itemsContainer);
        } else {
            newItem = Instantiate(_defaultItemPrefab, _itemsContainer);
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = itemSlot.amount.ToString();
        }
        newItem.transform.Find("ItemImage").GetComponent<Image>().sprite = itemSlot.item.inventoryIcon;
        newItem.GetComponent<DraggableItemUI>().item = itemSlot.item;
        itemsDisplayed.Add(itemSlot, newItem);
    }
}
