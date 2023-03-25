using System;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public static Action OnWeaponCollidedWithEnemy;
    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Enemy")) {
            OnWeaponCollidedWithEnemy?.Invoke();
        }    
    }
}
