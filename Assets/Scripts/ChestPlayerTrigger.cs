using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestPlayerTrigger : MonoBehaviour
{
    private Chest chest;

    void Awake()
    {
        chest = GetComponentInParent<Chest>();
    }

    void OnTriggerEnter(Collider other)
    {
        chest.playerInsideTrigger = true;
    }

    void OnTriggerExit(Collider other)
    {
        chest.playerInsideTrigger = false;
    }


}
