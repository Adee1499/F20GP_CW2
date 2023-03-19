using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string DisplayName;
    [TextArea(3, 5)]
    public string Description;
    public Sprite Icon;
}
