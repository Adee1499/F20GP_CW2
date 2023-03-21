using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] GameObject _nakedBodyPrefab;
    [SerializeField] GameObject _nakedHandsPrefab;
    [SerializeField] GameObject _nakedLegsPrefab;
    [SerializeField] GameObject _nakedBootsPrefab;

    void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();
    }

    public void EquipItem(GameObject prefab, EquipmentSlot slot)
    {
        Transform itemParent = GetSlotDefaults(slot).itemParent;
        if (itemParent.childCount > 0) {
            UnequipItem(slot);
        }
        RemoveSlotChildren(itemParent);
        InstantiateSlotObject(prefab, itemParent);
    }

    public void UnequipItem(EquipmentSlot slot)
    {
        (GameObject defaultSlotObject, Transform itemParent) = GetSlotDefaults(slot);
        RemoveSlotChildren(itemParent);
        if (defaultSlotObject != null) {
            InstantiateSlotObject(defaultSlotObject, itemParent);
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

    private void InstantiateSlotObject(GameObject prefab, Transform itemParent)
    {
        // Instantiate GameObject and parent under the slot
        GameObject armor = Instantiate(prefab, transform.position, Quaternion.identity);
        armor.SetActive(true);
        armor.transform.SetParent(itemParent);
        armor.GetComponent<SkinnedMeshRenderer>().CopyBonesFrom(GetComponentInChildren<SkinnedMeshRenderer>());
        _animator.Rebind();
    }
}