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
    [SerializeField] GameObject _arrowUp;
    [SerializeField] GameObject _arrowDown;

    Color lightgray = new Color(0.5660378f, 0.5660378f, 0.5660378f);
    Color crimsonRed = new Color(0.6037736f, 0.05354216f, 0.05354216f);
    Color darkGreen = new Color(0.05366678f, 0.4245283f, 0.1015867f);

    public void UpdateTooltip(InventoryItem item)
    {
        UpdateTooltip(item, 1f);
    }

    public void UpdateTooltip(InventoryItem item, float merchantMarkup)
    {
        _arrowUp.SetActive(false);
        _arrowDown.SetActive(false);
        _goldValue.text = Mathf.Floor(item.sellValue * merchantMarkup).ToString();
        _name.text = item.itemName;
        _description.text = item.description;

        // Try to cast as EquipmentItem
        EquipmentItem eqItem = item as EquipmentItem;
        if (eqItem != null) {
            // _rarity.text = eqItem.rarity;
            _levelRequired.text = $"Level required: {eqItem.levelRequired}";
            _levelRequired.color = XPSystem.Instance.GetCurrentLevel() >= eqItem.levelRequired
                ? lightgray
                : crimsonRed;

            _type.text = GetItemType(eqItem.equipmentSlot);
            int currentStatLower = 0; // Lower -1, same 0, higher +1
            if (eqItem.equipmentSlot == EquipmentSlot.Weapon) {
                _mainStatValue.text = eqItem.attackValue.ToString();
                _mainStatName.text = "Attack";
                currentStatLower = InventoryUI.Instance.attackStat.CompareTo(eqItem.attackValue);
            } else {
                _mainStatValue.text = eqItem.defenseValue.ToString();
                _mainStatName.text = "Defence";   
                currentStatLower = InventoryUI.Instance.defenceStat.CompareTo(eqItem.defenseValue);
            }
            Color newColor = lightgray;
            if (currentStatLower < 0) {
                // If current stat is lower than this item
                newColor = darkGreen;
                _arrowUp.SetActive(true);
            } else if (currentStatLower > 0) {
                newColor = crimsonRed;
                _arrowDown.SetActive(true);
            }
            _mainStatValue.color = newColor;
            _mainStatName.color = newColor;
        } else {
            PotionItem potItem = item as PotionItem;
            if (potItem != null) {
                _type.text = "Potion";
                _levelRequired.text = string.Empty;
                switch (potItem.EffectType) {
                    case PotionEffect.RestoreHealth:
                        _mainStatName.text = "Health";
                        break;
                    case PotionEffect.RestoreMana:
                        _mainStatName.text = "Mana";
                        break;
                    case PotionEffect.Default:
                        _mainStatName.text = string.Empty;
                        break;
                }
                _mainStatValue.text = potItem.EffectValue.ToString();
                _rarity.text = string.Empty;
            }
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
