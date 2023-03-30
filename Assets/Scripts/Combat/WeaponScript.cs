using System;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public static Action OnWeaponCollidedWithEnemy;
    
    [SerializeField] GameObject weapon;

    void Awake() {
        weapon = this.gameObject;
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Enemy")) {
            MeleeEnemyController enemy = other.GetComponent<MeleeEnemyController>();
            enemy.TakeDamage(10, 10, Vector3.Scale(weapon.transform.forward,weapon.transform.right));
            OnWeaponCollidedWithEnemy?.Invoke();
        }    
    }
}
