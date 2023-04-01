using System;
using UnityEngine;

public enum PotionEffect {
    Default,
    RestoreHealth,
    RestoreMana
}

[CreateAssetMenu(fileName = "Potion_", menuName = "Inventory System/Items/Potion Item")]
public class PotionItem : InventoryItem 
{
    public PotionEffect EffectType;
    public int EffectValue;
    public static Action<PotionEffect, int> OnPotionConsumed;
    
    public void Awake()
    {
        itemType = ItemType.Potion;
    }

    public void ConsumePotion() 
    {
        OnPotionConsumed?.Invoke(EffectType, EffectValue);
    }
}
