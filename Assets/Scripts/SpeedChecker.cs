using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChecker : MonoBehaviour
{
    public int speedLimit;

    [SerializeField] PopupType popupType = PopupType.PROMPT;
    private float speed;
    private const float speedMax = 120f;

    // amount of allowable extra in speed
    // e.g. limit = 20 and leeway = 3 => warn at speed 23
    public float speedLeeway = 3f;
    public int progress_order = 0;
    public int progress_total = 0;

    void Awake() {
        speed = 0f;
    }

    void OnTriggerStay (Collider other) {
        speed = GameManager.Instance.getBikeSpeed();
        if (speed > speedMax) speed = speedMax;
        
        if (speed > speedLimit+speedLeeway){
            Debug.Log($"Exceeded speed limit! ({speed})");
            string title = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "speedLimitErrorTitle");
            string text = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "speedLimitErrorText");
            Debug.Log($"title: ({title})");
            Debug.Log($"text: ({text})");
            GameManager.Instance.PopupSystem.popWithType(popupType,
                string.Format(title, speedLimit),
                text,
                true    // countAsError
            );

            GameManager.Instance.setErrorReason(Metrocycle.ErrorReason.OVERSPEEDING);

            // HACK: Kill velocity so that driver won't be flagged for overspeeding again immediately on resume
            GameManager.Instance.stopBike();
        }

        if (progress_order > 0 && progress_total > 0) {
            GameManager.Instance.updateProgressBar(progress_order, progress_total);
        }
    }
}
