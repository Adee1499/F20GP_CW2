using System.Collections;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public InventoryItem objRef;

    Collider _collider;
    Rigidbody _rigidbody;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        Physics.IgnoreLayerCollision(6, 7, true);
    }

    void Start()
    {
        _rigidbody.AddTorque(transform.right * Random.Range(0f, 0.5f), ForceMode.Impulse);
        _rigidbody.AddForce(new Vector3(Random.Range(-2f, 2f), Random.Range(2.5f, 3.5f), Random.Range(-2f, 2f)), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision other)
    {
        // Layer 3: Static
        if (other.gameObject.layer == 3) {
            StartCoroutine(DisableRigidbody());
        }
    }

    IEnumerator DisableRigidbody()
    {
        yield return new WaitForSeconds(2f);
            if (_rigidbody != null)
                Destroy(_rigidbody);
            _collider.isTrigger = true;
            Physics.IgnoreLayerCollision(6, 7, false);
    }

    void OnTriggerEnter(Collider other) 
    {
        Debug.Log($"Collided with {other.name}");
    }

    public void destroyObject(){
        Destroy(gameObject);
    }
}
