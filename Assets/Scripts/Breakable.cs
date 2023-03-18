using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject fracturedMesh;
    public float breakForce = 0.5f;
    public float breakRadius = 2.0f;

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
        //  FOR TESTING PURPOSES
        if(Input.GetKeyDown("f")) {
            Fracture();
        }
        
    }

    public void Fracture(float? force = null)
    {
        GameObject fracturedPot = Instantiate(fracturedMesh, transform.position, transform.rotation);

        foreach(Rigidbody body in fracturedPot.GetComponentsInChildren<Rigidbody>()) {
            body.AddExplosionForce(force ?? breakForce, transform.position, breakRadius, 3.0f);
        }

        Destroy(gameObject);
    }

}
