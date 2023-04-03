using System;
using UnityEngine;
using UnityEngine.Serialization;

public class AOESpellController : MonoBehaviour 
{
    public AOESpell aoeSpell;
    public static Action<float, GameObject> OnAOECollision;
    void OnTriggerEnter(Collider other) 
    {
        if (!other.name.Equals(gameObject.name) && !other.CompareTag("Merchant") && !other.CompareTag("Player")) {
            // Deal damage
            if (other.CompareTag("Enemy")) {
                OnAOECollision?.Invoke(aoeSpell.damage, other.gameObject);
            } else if (other.CompareTag("Breakable")) {
                other.GetComponent<Breakable>().Fracture();
            } else if (other.CompareTag("Explodeable")) {
                other.GetComponent<Explodeable>().Explode();
            }
        }
    }
}
