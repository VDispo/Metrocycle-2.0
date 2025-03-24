using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CheckpointDetection : MonoBehaviour
{
    public GameObject pairCollider; // activated after collision

    public bool deactivateAfterCollision;

    public bool showPopup;
    public PopupType popupType;
    public TMP_Text objectiveTextUI;
    public string PopupTitle { get; set; }
    public string PopupText { get; set; }
    public string ObjectiveText { get; set; }

    public UnityEvent callback = null;
    public UnityEvent loadStateCallback = null;
    [HideInInspector]
    public UnityEvent triggerSignal;
    public int progress_order = 0;
    public int progress_total = 0;

    void Awake() {
        triggerSignal = new UnityEvent();
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered collision with " + other.gameObject.name);

        if (pairCollider != null) {
            pairCollider.SetActive(true);
        }

        if (deactivateAfterCollision) {
            gameObject.SetActive(false);
        }

        if (showPopup) {
            GameManager.Instance.PopupSystem.popWithType(popupType, PopupTitle, PopupText);
            objectiveTextUI.text = ObjectiveText;
        }

        callback?.Invoke();

        triggerSignal.Invoke();

        if (progress_order > 0 && progress_total > 0) {
            GameManager.Instance.updateProgressBar(progress_order, progress_total);
        }
    }
}
