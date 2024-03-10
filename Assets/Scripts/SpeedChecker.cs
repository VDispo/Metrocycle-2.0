using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChecker : MonoBehaviour
{
    public GameObject bike;
    public bool activatePopup;
    public GameObject popup; // activated after collision
    public int speedLimit;
    private float speed;
    private float speedMax;
    Rigidbody rb;

    void OnTriggerStay (Collider other) {
        rb = bike.GetComponent<Rigidbody>();
        speed = 0f;
        speedMax = 120f;
        Vector3 vel = rb.velocity;

        speed = vel.magnitude*3;
        if (speed > speedMax) speed = speedMax;
        
        if (speed > speedLimit){
            popup.SetActive(true);
        }
    }
    
}
