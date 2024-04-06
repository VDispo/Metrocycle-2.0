using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForbiddenLaneChangeCheck : MonoBehaviour
{
    public GameObject errorPopup;
    public GameObject errorText;


    private bool hasCrossedLine;
    private TextMeshProUGUI textElement;

    void Start() {
        hasCrossedLine = false;
        textElement = errorText.GetComponent<TextMeshProUGUI>();
    }

    public void enteredLane(GameObject lane) {
        if (hasCrossedLine) {
            Debug.Log("Illegal lane change!!!!");
            textElement.text = "You are not allowed to change lane anymore when the line is solid.";

            errorPopup.SetActive(true);
        }

        hasCrossedLine = false;
    }

    void OnTriggerEnter (Collider other) {
        hasCrossedLine = true;
    }
}
