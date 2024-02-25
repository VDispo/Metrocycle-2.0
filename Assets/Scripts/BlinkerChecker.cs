using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkerCheck : MonoBehaviour
{
    public bool hasPairCollider;
    public GameObject pairCollider; // activated after collision

    public bool deactivateAfterCollision;

    public bool activatePopup;
    public GameObject popup; // activated after collision

    public bool rightTurn; //false if left, true if right
    private int blinkerStatus;


    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered collision with " + other.gameObject.name);
        if (hasPairCollider) {
            pairCollider.SetActive(true);
        }

        if (deactivateAfterCollision) {
            gameObject.SetActive(false);
        }

        //Transform childTransform = parentObject.transform.Find("Dashboard Canvas/Blinkers");
        //blinkerStatus = childTransform.rightStatus;
        if (rightTurn && blinkerStatus == 0) {
            popup.SetActive(true);
        }
    }
}
