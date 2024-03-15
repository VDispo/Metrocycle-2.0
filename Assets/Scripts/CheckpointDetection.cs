using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckpointDetection : MonoBehaviour
{
    public bool hasPairCollider;
    public GameObject pairCollider; // activated after collision

    public bool deactivateAfterCollision;

    public bool activatePopup;
    public GameObject popup; // activated after collision

    public bool changePopupText;
    [TextArea(3, 10)] public string popupText;

    private TextMeshProUGUI textElement;

    // Start is called before the first frame update
    void Start()
    {
        textElement = popup.transform.Find("Instructions").GetComponent<TextMeshProUGUI>();
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered collision with " + other.gameObject.name);
        if (hasPairCollider) {
            pairCollider.SetActive(true);
        }

        if (deactivateAfterCollision) {
            gameObject.SetActive(false);
        }

        if (activatePopup) {
            if (changePopupText) {
                textElement.text = popupText;
            }

            popup.SetActive(true);
        }
    }
}
