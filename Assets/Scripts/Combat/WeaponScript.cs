using System;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public static Action<float, EnemyController, Vector3> OnWeaponCollidedWithEnemy;
    float tempDamageVal = 10;

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Enemy")) {
            EnemyController enemyRef = other.GetComponent<EnemyController>();

            // get the direction of impact
            Vector3 differential = enemyRef.transform.position - this.transform.position;
            OnWeaponCollidedWithEnemy?.Invoke(tempDamageVal, enemyRef, differential.normalized);
        }    
    }
}
