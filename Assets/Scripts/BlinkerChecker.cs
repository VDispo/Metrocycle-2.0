using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkerCheck : MonoBehaviour
{
    public bool activatePopup;
    public GameObject popup; // activated after collision

    public Blinker whichBlinker; //false if left, true if right
    public GameObject blinkers;
    private blinkers blinkerScript;

    // Start is called before the first frame update
    void Start()
    {
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();
    }

    void OnTriggerEnter (Collider other) {
        if (whichBlinker == Blinker.RIGHT && blinkerScript.rightStatus == 0) {
            popup.SetActive(true);
        }
        if (whichBlinker == Blinker.LEFT && blinkerScript.leftStatus == 0) {
            popup.SetActive(true);
        }
    }
}
