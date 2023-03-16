using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobeTest : MonoBehaviour
{
    public Slider healthSlider;
    public float refillSpeed;
    public bool refilling;
    
    void Start()
    {
        healthSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if(refilling) {
            
            // refill at defined speed
            healthSlider.value = healthSlider.value + (refillSpeed * Time.deltaTime);
            
            // when globe full, refilling off
            if(healthSlider.value == 1) {
                refilling = false;   
            }
        }
        
        // left click to lose fluid
        if(Input.GetMouseButtonDown (0)) {
            healthSlider.value = healthSlider.value - 0.1f;   
        }
        
        // right click for refilling
        if(Input.GetMouseButtonDown (1)) {
            refilling = true;   
        }
    }
}
