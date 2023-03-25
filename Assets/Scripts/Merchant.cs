using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Merchant : MonoBehaviour
{
    private static Merchant _instance;
    public static Merchant Instance { get { return _instance; }}
    [SerializeField] AudioClip[] _greetingClips;
    AudioSource _audioSource;
    public Inventory inventory;
    Dictionary<InventorySlot, GameObject> _itemsDisplayed = new Dictionary<InventorySlot, GameObject>();
    GameObject _merchantUI;
    Transform _merchantItemsContainer;
    GameObject _merchantItemPrefab;
    [Range(1.0f,2.0f)]
    [SerializeField] float _merchantMarkup;
    public float MarkupValue { get { return _merchantMarkup; }}
    TextMeshProUGUI _goldAmount;
    
    void Awake()
    {
        _instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        _merchantUI = InventoryUI.Instance.UI_Merchant;
        _goldAmount = _merchantUI.transform.Find("Gold").GetComponentInChildren<TextMeshProUGUI>();
        _merchantItemPrefab = InventoryUI.Instance.MerchantItemPrefab;
        _merchantItemsContainer = _merchantUI.transform.Find("ScrollView").GetComponent<ScrollRect>().content;
        PopulateInventory();
    }

    void Update()
    {
        RefreshInventory();
    }

    void PopulateInventory()
    {
        for (int i = 0; i < inventory.items.Count; i++) {
            InsertInventoryItem(inventory.items[i]);
        }
    }

    void RefreshInventory()
    {
        _goldAmount.text = inventory.gold.ToString();
        for (int i = 0; i < inventory.items.Count; i++) {
            if (_itemsDisplayed.ContainsKey(inventory.items[i])) {
                if (_itemsDisplayed[inventory.items[i]].GetComponent<DraggableItemUI>().item.itemType != ItemType.Equipment) {
                    _itemsDisplayed[inventory.items[i]].GetComponentInChildren<TextMeshProUGUI>().text = inventory.items[i].amount.ToString();
                }
            } else {
                InsertInventoryItem(inventory.items[i]);
            }
        }
    }

    void InsertInventoryItem(InventorySlot itemSlot)
    {
        GameObject newItem = Instantiate(_merchantItemPrefab, _merchantItemsContainer);
        if (itemSlot.item.itemType == ItemType.Equipment) {
            newItem.transform.Find("ItemImage").GetChild(0).gameObject.SetActive(false); // Disable amount text on the prefab
        } else {
            newItem.transform.Find("ItemImage").GetComponentInChildren<TextMeshProUGUI>().text = itemSlot.amount.ToString();
        }
        newItem.transform.Find("ItemImage").GetComponent<Image>().sprite = itemSlot.item.inventoryIcon;
        newItem.GetComponent<DraggableItemUI>().item = itemSlot.item;
        newItem.transform.Find("Gold").GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Floor(itemSlot.item.sellValue * _merchantMarkup).ToString();
        newItem.transform.Find("BuyButton").GetComponent<Button>().onClick.AddListener(SellItemToPlayer);
        _itemsDisplayed.Add(itemSlot, newItem);
    }

    void RemoveInventoryItem(InventoryItem item)
    {
        foreach(InventorySlot slot in inventory.items) {
            if (slot.CompareItem(item)) {
                inventory.items.Remove(slot);
                GameObject inventoryObject = _itemsDisplayed[slot];
                _itemsDisplayed.Remove(slot);
                Destroy(inventoryObject);
                RefreshInventory();
                break;
            }
        }
    }

    public void BuyItemFromPlayer(InventoryItem item)
    {
        InventorySlot newItem = new InventorySlot(item, 1);
        inventory.items.Add(newItem);
        inventory.gold -= item.sellValue;
        InventoryUI.Instance.inventory.gold += item.sellValue;
    }

    void SellItemToPlayer()
    {
        DraggableItemUI targetItem = EventSystem.current.currentSelectedGameObject.GetComponentInParent<DraggableItemUI>();
        // If player has enough gold to buy this item
        int buyValue = (int)Mathf.Floor(targetItem.item.sellValue * _merchantMarkup);
        if (InventoryUI.Instance.inventory.gold >= buyValue) {
            // Add item to player's inventory
            InventoryUI.Instance.BuyItemFromMerchant(targetItem.item);
            // Remove item from merchant's inventory
            RemoveInventoryItem(targetItem.item);

            // Trade gold
            inventory.gold += buyValue;
            InventoryUI.Instance.inventory.gold -= buyValue;
        }
    }

    private void ClearInventory()
    {
        for(int i = 0; i < _merchantItemsContainer.childCount; i++) {
            Destroy(_merchantItemsContainer.GetChild(i).gameObject);
        }
    }

    void InviteToTrade()
    {
        _audioSource.clip = _greetingClips[Random.Range(0, _greetingClips.Length)];
        _audioSource.Play();
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player")) {
            if (!_audioSource.isPlaying)
                InviteToTrade();
        }    
    }
}
