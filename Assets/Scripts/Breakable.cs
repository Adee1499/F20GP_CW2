using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject fracturedMesh;
    public float breakForce = 0.5f;
    public float breakRadius = 2.0f;
    public LayerMask layermask;

    public int suggestedLevel = 1;
    public int numberOfLoot = 1;
    public float rarityModifier = 0.1f;

    private Outline outline;

    void Awake() 
    {
        outline = gameObject.GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
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
        
    }

    public void Fracture(float? force = null)
    {
        GameObject fracturedPot = Instantiate(fracturedMesh, transform.position, transform.rotation);

        foreach(Rigidbody body in fracturedPot.GetComponentsInChildren<Rigidbody>()) {
            body.AddExplosionForce(force ?? breakForce, transform.position, breakRadius, 3.0f);
        }

        LootManager lootManager = FindObjectOfType<LootManager>();
        lootManager.DropLoot(transform.position, numberOfLoot, suggestedLevel, rarityModifier);

        Destroy(gameObject);
    }

}
