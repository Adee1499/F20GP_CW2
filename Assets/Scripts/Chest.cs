using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    public GameObject openChestPrefab;
    public bool playerInsideTrigger = false;
    private Outline outline;

    void Awake()
    {
        outline = gameObject.GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 0;
    }

    void OnMouseOver()
    {
        outline.OutlineWidth = 7;
    }

    void OnMouseExit()
    {
        outline.OutlineWidth = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("f") && playerInsideTrigger){
            Instantiate(openChestPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
