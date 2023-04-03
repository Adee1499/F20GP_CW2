using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public List<EquipmentItem> DroppableLoot;
    public Dictionary<string, LootRarity> RarityStats = new Dictionary<string, LootRarity>();
    public GameObject GoldPrefab;

    void Awake()
    {
        RarityStats["Common"] = new LootRarity("Common", new Color(1.0f, 1.0f, 1.0f), 1.0f);
        RarityStats["Uncommon"] = new LootRarity("Uncommon", new Color(0.19f, 0.97f, 0.15f), 1.4f);
        RarityStats["Rare"] = new LootRarity("Rare", new Color(0.84f, 0.32f, 0.97f), 1.8f);
        RarityStats["Legendary"] = new LootRarity("Legendary", new Color(1.0f, 0.72f, 0.17f), 2.5f);

    }

    public void DropLoot(Vector3 position, int noItemsDropped, int suggestedLevel, float rarityModifier = 1.0f)
    {
        List<GameObject> prefabs = new List<GameObject>();
        for(int i = 0; i < noItemsDropped; i++){
            var item = DroppableLoot[Random.Range(0, DroppableLoot.Count - 1)];
            var newItem = Instantiate(item);

            LootRarity itemRarity = GenerateRarity(rarityModifier);

            var prefab = newItem.onGroundPrefab;
            var prefabOutline = prefab.GetComponent<Outline>(); 
            prefabOutline.OutlineColor = RarityStats[itemRarity.rarity].rarityColour;
            prefabOutline.OutlineWidth = 3f;

            // 20% chance that common items are a level below
            if(suggestedLevel > 1 && itemRarity.rarity == "Common" && Random.Range(0.0f, 1.0f) < 0.2f)
                suggestedLevel -= 1;

            newItem.levelRequired = suggestedLevel;

            if(newItem.equipmentSlot == EquipmentSlot.Weapon){
                newItem.attackValue = (int) Mathf.Round(newItem.attackValue * suggestedLevel * itemRarity.statModifier * Random.Range(0.8f, 1.2f));
            }
            else {
                newItem.defenseValue = (int) Mathf.Round(newItem.defenseValue * suggestedLevel * itemRarity.statModifier * Random.Range(0.8f, 1.2f));
            }

            newItem.sellValue = (int) Mathf.Round(newItem.sellValue * newItem.defenseValue * itemRarity.statModifier);
            newItem.lootRarity = itemRarity;

            prefab.GetComponent<Loot>().objRef = newItem;

            if (newItem.equipmentSlot == EquipmentSlot.Weapon) {
                Instantiate(prefab, position + Vector3.up * 0.5f, Quaternion.identity);
            } else {
                Instantiate(prefab, position, Quaternion.identity);
            }

            prefabs.Add(prefab);
        }

        foreach(GameObject obj in prefabs){
            obj.GetComponent<Rigidbody>().AddExplosionForce(5.0f, transform.position, 5.0f, 10.0f);
        }
        
    }

    private LootRarity GenerateRarity(float rarityModifier) {
        float rand = Random.Range(0.0f, 1.0f);

        if(rand < 0.05f * rarityModifier){
            return RarityStats["Legendary"];
        }
        else if(rand < 0.2f * rarityModifier){
            return RarityStats["Rare"];
        }
        else if(rand < 0.5f * rarityModifier){
            return RarityStats["Uncommon"];
        }
        else {
            return RarityStats["Common"]; // SHOULD PROBABLY PRODUCE ANOTHER CHANCE FOR GOLD INSTEAD OF COMMON ITEM
        }
    }
}

public class LootRarity {
    public string rarity;
    public Color rarityColour;
    public float statModifier;

    public LootRarity(string _rarity, Color _rarityColour, float _statModifier) {
        rarity = _rarity;
        rarityColour = _rarityColour;
        statModifier = _statModifier;
    }
}
