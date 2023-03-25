using TMPro;
using UnityEngine;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _type;
    [SerializeField] TextMeshProUGUI _description;
    [SerializeField] TextMeshProUGUI _rarity;
    [SerializeField] TextMeshProUGUI _goldValue;
    [SerializeField] TextMeshProUGUI _mainStatValue;
    [SerializeField] TextMeshProUGUI _mainStatName;
    [SerializeField] TextMeshProUGUI _levelRequired;

    public void UpdateTooltip(InventoryItem item)
    {
        UpdateTooltip(item, 1f);
    }

    public void UpdateTooltip(InventoryItem item, float merchantMarkup)
    {
        _goldValue.text = Mathf.Floor(item.sellValue * merchantMarkup).ToString();
        _name.text = item.itemName;
        _description.text = item.description;

        // Try to cast as EquipmentItem
        EquipmentItem eqItem = item as EquipmentItem;
        if (eqItem != null) {
            // _rarity.text = eqItem.rarity;
            _mainStatValue.text = eqItem.defenseValue.ToString();
            _levelRequired.text = $"Level required: {eqItem.levelRequired}";

            _type.text = GetItemType(eqItem.equipmentSlot);
            string mainStatName = eqItem.equipmentSlot == EquipmentSlot.Weapon ? "Attack" : "Defence";
            _mainStatName.text = mainStatName;
        } else {
            _type.text = "Potion";
        }
    }

    private string GetItemType(EquipmentSlot slot)
    {
        switch(slot) {
            case EquipmentSlot.Helmet:
                return "Headgear";
            case EquipmentSlot.Body:
                return "Chest armor";
            case EquipmentSlot.Hands:
                return "Gauntlets";
            case EquipmentSlot.Legs:
                return "Trousers";
            case EquipmentSlot.Boots:
                return "Boots";
            case EquipmentSlot.Cape:
                return "Cape";
            case EquipmentSlot.Weapon:
                return "One-handed weapon";
            default:
                return slot.ToString();
        }
    }
}
