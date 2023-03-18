using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodeable : MonoBehaviour
{
    public GameObject fracturedMesh;
    public float explosionForce = 100.0f;
    public float explosionRadius = 10.0f;
    public GameObject particles;
    private ParticleSystem ps;

    private Outline outline;

    void Awake() 
    {
        outline = gameObject.GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 0;
        ps = particles.GetComponent<ParticleSystem>();
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
            Explode();
        }
        
    }

    public void Explode()
    {
        GameObject fracturedObj = Instantiate(fracturedMesh, transform.position, transform.rotation);
        var nearbyObjects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach(var obj in nearbyObjects){
            var rb = obj.GetComponent<Rigidbody>();
            if(rb == null) continue;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        foreach(Rigidbody body in fracturedObj.GetComponentsInChildren<Rigidbody>()) {
            body.AddExplosionForce(explosionForce, transform.position, explosionRadius, 3.0f);
        }

        Instantiate(particles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

}
