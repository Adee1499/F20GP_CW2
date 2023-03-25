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
    [SerializeField] TextMeshProUGUI _goldAmount;
    [SerializeField] private GameObject _defaultItemPrefab;
    [SerializeField] private GameObject _equipmentItemPrefab;
    public GameObject EquipmentItemPrefab { get { return _equipmentItemPrefab; }}
    [SerializeField] private GameObject _merchantItemPrefab;
    public GameObject MerchantItemPrefab { get { return _merchantItemPrefab; }}
    [SerializeField] private GameObject _itemTooltip;
    public GameObject ItemTooltip { get { return _itemTooltip; }}
    [HideInInspector] public GameObject CurrentItem;
    Dictionary<InventorySlot, GameObject> itemsDisplayed = new Dictionary<InventorySlot, GameObject>();

    // UI Elements references
    public GameObject UI_Inventory;
    public GameObject UI_Equipment;
    public GameObject UI_Merchant;

    // Stats
    PlayerStateMachine _playerReference;

    public int attackStat;
    public int defenceStat;

    TextMeshProUGUI _attackStatText;
    TextMeshProUGUI _defenceStatText;
    TextMeshProUGUI _healthStatText;
    TextMeshProUGUI _manaStatText;

    void Awake()
    {
        _instance = this;
        attackStat = 0;
        defenceStat = 0;
        Transform UI_Stats = UI_Equipment.transform.Find("UI_Stats");
        _attackStatText = UI_Stats.Find("Stats_Attack").GetComponent<TextMeshProUGUI>();
        _defenceStatText = UI_Stats.Find("Stats_Armor").GetComponent<TextMeshProUGUI>();
        _healthStatText = UI_Stats.Find("Stats_HP").GetComponent<TextMeshProUGUI>();
        _manaStatText = UI_Stats.Find("Stats_MP").GetComponent<TextMeshProUGUI>();
        PopulateInventory();
    }

    void Start()
    {
        _playerReference = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
    }

    void Update()
    {
        RefreshInventory();
        RefreshStats();
    }

    public void PopulateInventory()
    {
        for (int i = 0; i < inventory.items.Count; i++) {
            InsertInventoryItem(inventory.items[i]);
        }
    }

    public void RefreshInventory()
    {
        _goldAmount.text = inventory.gold.ToString();
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
                GameObject inventoryObject = itemsDisplayed[slot];
                itemsDisplayed.Remove(slot);
                Destroy(inventoryObject);
                RefreshInventory();
                break;
            }
        }
    }

    public void BuyItemFromMerchant(InventoryItem item) 
    {
        InventorySlot newItem = new InventorySlot(item, 1);
        inventory.items.Add(newItem);
    }

    void RefreshStats()
    {
        _attackStatText.text = $"Attack: {attackStat}";
        _defenceStatText.text = $"Armor: {defenceStat}";
        _healthStatText.text = $"HP: {_playerReference.MaxPlayerHealth}";
        _manaStatText.text = $"HP: {_playerReference.MaxPlayerMana}";
    }
}