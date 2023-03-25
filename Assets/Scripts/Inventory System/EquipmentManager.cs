using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentManager : MonoBehaviour
{
    private static EquipmentManager _instance;
    public static EquipmentManager Instance { get { return _instance; }}
    private Animator _animator;
    [SerializeField] Transform _helmetSlot;
    [SerializeField] Transform _bodySlot;
    [SerializeField] Transform _handsSlot;
    [SerializeField] Transform _legsSlot;
    [SerializeField] Transform _bootsSlot;
    [SerializeField] Transform _capeSlot;
    [SerializeField] Transform _rightHandSlot;

    [SerializeField] GameObject _nakedBodyPrefab;
    [SerializeField] GameObject _nakedHandsPrefab;
    [SerializeField] GameObject _nakedLegsPrefab;
    [SerializeField] GameObject _nakedBootsPrefab;

    void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();
    }

    public void EquipItem(InventoryItem item)
    {
        Transform itemParent = GetSlotDefaults(item.equipmentSlot).itemParent;
        if (itemParent.childCount > 0) {
            UnequipItem(item.equipmentSlot);
        }
        RemoveSlotChildren(itemParent);
        InstantiateSlotObject(item.onPlayerPrefab, itemParent, item.equipmentSlot != EquipmentSlot.Weapon);
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        (GameObject defaultSlotObject, Transform itemParent) = GetSlotDefaults(slot);
        RemoveSlotChildren(itemParent);
        if (defaultSlotObject != null) {
            InstantiateSlotObject(defaultSlotObject, itemParent, slot != EquipmentSlot.Weapon);
        }
    }

    private (GameObject defaultSlotObject, Transform itemParent) GetSlotDefaults(EquipmentSlot slot)
    {
        GameObject defaultSlotObject = null;
        Transform itemParent = transform;
        switch (slot) {
            case EquipmentSlot.Helmet: 
                itemParent = _helmetSlot;
                break;
            case EquipmentSlot.Body: 
                defaultSlotObject = _nakedBodyPrefab;
                itemParent = _bodySlot;
                break;
            case EquipmentSlot.Hands: 
                defaultSlotObject = _nakedHandsPrefab;
                itemParent = _handsSlot;
                break;
            case EquipmentSlot.Legs: 
                defaultSlotObject = _nakedLegsPrefab;
                itemParent = _legsSlot;
                break;
            case EquipmentSlot.Boots: 
                defaultSlotObject = _nakedBootsPrefab;
                itemParent = _bootsSlot;
                break;
            case EquipmentSlot.Cape: 
                itemParent = _capeSlot;
                break;
            case EquipmentSlot.Weapon:
                itemParent = _rightHandSlot;
                break;
            default: 
                break;
        }
        return (defaultSlotObject, itemParent);
    }

    private void RemoveSlotChildren(Transform slot) 
    {
        // Remove any objects under the slot
        int childCount = slot.childCount;
        if (childCount > 0) {
            for (int i = 0; i < childCount; i++) {
                Destroy(slot.GetChild(i).gameObject);
            }
        }
    }

    private void InstantiateSlotObject(GameObject prefab, Transform itemParent, bool hasSkinnedMeshRenderer)
    {
        // Instantiate GameObject and parent under the slot
        GameObject item = Instantiate(prefab, transform.position, Quaternion.identity);
        item.SetActive(true);
        item.transform.SetParent(itemParent);
        if (hasSkinnedMeshRenderer) {
            item.GetComponent<SkinnedMeshRenderer>().CopyBonesFrom(GetComponentInChildren<SkinnedMeshRenderer>());
            _animator.Rebind();
        } else {
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
    }
}