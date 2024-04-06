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

    // amount of allowable extra in speed
    // e.g. limit = 20 and leeway = 3 => warn at speed 23
    private float speedLeeway = 3f;

    void OnTriggerStay (Collider other) {
        speed = 0f;
        speedMax = 120f;
        Vector3 vel = rb.velocity;

        speed = vel.magnitude*3;
        if (speed > speedMax) speed = speedMax;
        
        if (speed > speedLimit+speedLeeway){
            popup.SetActive(true);
        }
    }
    
}
