using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject fracturedMesh;
    public float breakForce = 0.5f;

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
            Vector3 appliedForce = (body.transform.position - transform.position).normalized * (force ?? breakForce);
            body.AddForce(appliedForce);
        }

        Destroy(gameObject);
    }
}
