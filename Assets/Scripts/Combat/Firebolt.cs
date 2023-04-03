using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : MonoBehaviour
{
    public float life = 3f; 
    public static Action<float> OnProjectileHitPlayer;
    Transform target;
    
    public void SetTarget(Transform target) {
        this.target = target;
    }

    private void FixedUpdate() {
        if(target)
            transform.position = Vector3.MoveTowards(transform.position, target.position + new Vector3(0,1f,0), 1f);
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("hit");
        if(other.gameObject.tag != "enemy"){
            if (other.gameObject.tag == "Player")
            {   
                Destroy(gameObject);
                OnProjectileHitPlayer?.Invoke(1f);
            }
        } else {
            Destroy(gameObject);
        }
    }
}
