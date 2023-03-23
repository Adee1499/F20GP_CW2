using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public List<EquipmentItem> DroppableLoot;
    public Dictionary<string, LootRarity> RarityStats = new Dictionary<string, LootRarity>();

    void Awake()
    {
        RarityStats["Common"] = new LootRarity("Common", new Color(1.0f, 1.0f, 1.0f), 1.0f);
        RarityStats["Uncommon"] = new LootRarity("Uncommon", new Color(0.19f, 0.97f, 0.15f), 1.2f);
        RarityStats["Rare"] = new LootRarity("Rare", new Color(0.84f, 0.32f, 0.97f), 1.7f);
        RarityStats["Legendary"] = new LootRarity("Legendary", new Color(1.0f, 0.72f, 0.17f), 2.0f);

    }

    public void DropLoot(Vector3 position, int noItemsDropped)
    {
        List<GameObject> prefabs = new List<GameObject>();
        for(int i = 0; i < noItemsDropped; i++){
            var item = DroppableLoot[Random.Range(0, DroppableLoot.Count - 1)];
            var newItem = Instantiate(item);

            LootRarity itemRarity = GenerateRarity();

            var prefab = newItem.onGroundPrefab;
            prefab.GetComponent<Outline>().OutlineColor = RarityStats[itemRarity.rarity].rarityColour;
            prefab.GetComponent<Loot>().objRef = newItem;

            if(newItem.itemType == ItemType.Equipment){
                newItem.defenseValue = (int) Mathf.Round(newItem.defenseValue * itemRarity.statModifier * Random.Range(0.9f, 1.1f));
                Debug.Log(newItem.defenseValue);
            }

            Instantiate(prefab, position, Quaternion.identity);
            prefabs.Add(prefab);
        }

        foreach(GameObject obj in prefabs){
            obj.GetComponent<Rigidbody>().AddExplosionForce(5.0f, transform.position, 5.0f);
        }
        
    }

    private LootRarity GenerateRarity() {
        float rand = Random.Range(0.0f, 1.0f);

        if(rand < 0.05f){
            return RarityStats["Legendary"];
        }
        else if(rand < 0.2f){
            return RarityStats["Rare"];
        }
        else if(rand < 0.5f){
            return RarityStats["Uncommon"];
        }
        else {
            return RarityStats["Common"];
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
