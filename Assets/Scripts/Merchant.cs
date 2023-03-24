using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Merchant : MonoBehaviour
{
    [SerializeField] AudioClip[] _greetingClips;
    AudioSource _audioSource;
    [SerializeField] Inventory _inventory;
    GameObject _merchantUI;
    Transform _merchantItemsContainer;
    GameObject _merchantItemPrefab;
    [Range(1.0f,2.0f)]
    [SerializeField] float _merchantMarkup;
    
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        PlayerInteractState.OnInteractWithMerchant += PopulateInventory;
    }

    void Start()
    {
        _merchantUI = InventoryUI.Instance.UI_Merchant;
        _merchantItemPrefab = InventoryUI.Instance.MerchantItemPrefab;
        _merchantItemsContainer = _merchantUI.transform.Find("ScrollView").GetComponent<ScrollRect>().content;
    }

    void PopulateInventory()
    {
        ClearInventory();
        _merchantUI.transform.Find("Gold").GetComponentInChildren<TextMeshProUGUI>().text = _inventory.gold.ToString();
        foreach (InventorySlot itemSlot in _inventory.items) {
            GameObject newItem = Instantiate(_merchantItemPrefab, _merchantItemsContainer);
            newItem.transform.Find("ItemImage").GetComponent<Image>().sprite = itemSlot.item.inventoryIcon;
            newItem.GetComponent<DraggableItemUI>().item = itemSlot.item;
            newItem.transform.Find("Gold").GetComponentInChildren<TextMeshProUGUI>().text = (itemSlot.item.sellValue * _merchantMarkup).ToString();
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
