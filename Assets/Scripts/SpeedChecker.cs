using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChecker : MonoBehaviour
{
    public int speedLimit;
    public string popupText = null;
    public string popupTitle = null;

    [SerializeField] PopupType popupType = PopupType.PROMPT;
    private float speed;
    private const float speedMax = 120f;

    // amount of allowable extra in speed
    // e.g. limit = 20 and leeway = 3 => warn at speed 23
    public float speedLeeway = 3f;

    void Awake() {
        speed = 0f;
    }

    void OnTriggerStay (Collider other) {
        speed = GameManager.Instance.getBikeSpeed();
        if (speed > speedMax) speed = speedMax;
        
        if (speed > speedLimit+speedLeeway){
            Debug.Log("Exceeded speed limit!");
            string text = (popupText ?? "") == "" ? "Make sure to keep an eye on your speedometer." : popupText;
            string title = (popupTitle ?? "") == "" ? $"You have exceeded the {speedLimit} kph speed limit!" : popupTitle;
            GameManager.Instance.PopupSystem.popWithType(popupType,
                title,
                text,
                true    // countAsError
            );

            // HACK: Kill velocity so that driver won't be flagged for overspeeding again immediately on resume
            GameManager.Instance.stopBike();
        }
    }
}
