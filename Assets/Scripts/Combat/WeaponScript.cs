using System;
using UnityEngine;

public class WeaponScript : MonoBehaviour 
{
    public EquipmentItem weaponSO;
    public static bool CanDealDamage = false;
    public static Action<float, EnemyController, Vector3> OnWeaponCollidedWithEnemy;

    void OnTriggerEnter(Collider other) 
    {
        if (!CanDealDamage) return;
        if (other.CompareTag("Enemy")) {
            EnemyController enemyRef = other.GetComponent<EnemyController>();
            // get the direction of impact
            Vector3 differential = enemyRef.transform.position - this.transform.position;
            OnWeaponCollidedWithEnemy?.Invoke(weaponSO.attackValue, enemyRef, differential.normalized);
        } else if (other.CompareTag("Breakable")) {
            other.GetComponent<Breakable>().Fracture();
        } else if (other.CompareTag("Explodeable")) {
            other.GetComponent<Explodeable>().Explode();
        }
    }
}
