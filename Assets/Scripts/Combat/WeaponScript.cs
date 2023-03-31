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
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            Vector3 contactNormal = transform.position - contactPoint;

            OnWeaponCollidedWithEnemy?.Invoke(tempDamageVal, enemyRef, contactPoint + contactNormal * 0f);
        }    
    }
}
