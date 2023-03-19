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
    public LayerMask layermask;

    private Outline outline;

    void Awake() 
    {
        outline = gameObject.GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 0;
        ps = particles.GetComponent<ParticleSystem>();
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

    public void Explode()
    {
        GameObject fracturedObj = Instantiate(fracturedMesh, transform.position, transform.rotation);
        var nearbyBreakables = Physics.OverlapSphere(transform.position, explosionRadius);

        // first check for nearby breakables (breakables don't contain rigidbodies until fractured)
        foreach(var obj in nearbyBreakables){
            var breakable = obj.GetComponent<Breakable>();

            if(breakable == null) continue;

            breakable.Fracture();
        }

        // must search again to include rigidbodies from nearby breakables
        var nearbyObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        // check for remaining rigidbodies
        foreach(var obj in nearbyObjects){
            var rb = obj.GetComponent<Rigidbody>();
            if(rb == null) continue;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        Instantiate(particles, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

}
