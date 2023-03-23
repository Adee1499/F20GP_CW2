using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    public GameObject openChestPrefab;
    public bool playerInsideTrigger = false;
    private Outline outline;
    public LayerMask layermask;

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
        LootManager lootManager = FindObjectOfType<LootManager>();
        Instantiate(openChestPrefab, transform.position, transform.rotation);
        lootManager.DropLoot(transform.position, 3);
        Destroy(gameObject);
    }

}
