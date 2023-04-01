using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {
    [SerializeField] private Inventory _chestInventory;
    public GameObject openChestPrefab;
    public bool playerInsideTrigger = false;
    private Outline outline;
    public LayerMask layermask;

    public int suggestedLevel = 1;
    public int numberOfLoot = 3;
    public float rarityModifier = 1.0f;

    void Awake()
    {
        outline = gameObject.GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, 1000, layermask)) {
            if(hit.transform.position == transform.position){
                outline.OutlineWidth = 5;
                outline.OutlineColor = new Color(1.0f, 1.0f, 1.0f);
            }
            else {
                outline.OutlineWidth = 0;
            }
        }
        else {
            outline.OutlineWidth = 0;
        }

        if(playerInsideTrigger){

            outline.OutlineWidth = 7;
            outline.OutlineColor = new Color(1.0f, 0.5f, 0.0f);

            if(Input.GetKeyDown("f")){
                Open();
            }
        }
        
    }

    void Open() {
        if (_chestInventory != null && _chestInventory.items.Count > 0) {
            for (int i = 0; i < _chestInventory.items.Count; i++) {
                var item = _chestInventory.items[i].item;
                var prefab = item.onGroundPrefab;
                prefab.GetComponent<Outline>().OutlineColor = Color.white;
                prefab.GetComponent<Outline>().OutlineWidth = 2f;

                if (prefab.GetComponent<Loot>().objRef == null)
                    prefab.GetComponent<Loot>().objRef = item as EquipmentItem;
                var obj = Instantiate(prefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                obj.GetComponent<Rigidbody>().AddExplosionForce(5.0f, transform.position, 5.0f);
            }
            Instantiate(openChestPrefab, transform.position, transform.rotation);
        }
        else {
            LootManager lootManager = FindObjectOfType<LootManager>();
            Instantiate(openChestPrefab, transform.position, transform.rotation);
            lootManager.DropLoot(transform.position, numberOfLoot, suggestedLevel, rarityModifier);
        }
        Destroy(gameObject);
    }

}
