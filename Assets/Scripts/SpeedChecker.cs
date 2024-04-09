using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChecker : MonoBehaviour
{
    public GameObject bike = null;
    public bool activatePopup;
    public GameObject popup; // activated after collision
    public int speedLimit;
    private float speed;
    private float speedMax;

    // amount of allowable extra in speed
    // e.g. limit = 20 and leeway = 3 => warn at speed 23
    public float speedLeeway = 3f;

    void Awake() {
        if (bike == null) {
            bike = GameManager.Instance.bike;
        }
        speed = 0f;
    }

    void OnTriggerStay (Collider other) {
        speedMax = 120f;

        speed = GameManager.Instance.getBikeSpeed();
        if (speed > speedMax) speed = speedMax;
        
        if (speed > speedLimit+speedLeeway){
            Debug.Log("Exceeded speed limit!");
            popup.SetActive(true);
        }
    }
}
