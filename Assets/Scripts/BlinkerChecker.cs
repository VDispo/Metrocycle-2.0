using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkerCheck : MonoBehaviour
{
    public bool activatePopup;
    public GameObject popup; // activated after collision

    public bool rightTurn; //false if left, true if right
    public GameObject blinkers;
    private blinkers blinkerScript;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter (Collider other) {

        blinkerScript = blinkers.GetComponent<blinkers>();
        if (rightTurn && blinkerScript.rightStatus == 0) {
            popup.SetActive(true);
        }
        if (!rightTurn && blinkerScript.leftStatus == 0) {
            popup.SetActive(true);
        }
    }
}
