using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeLaneChecker : MonoBehaviour
{
    public GameObject motorbike;
    public GameObject popup;
    public float minBlinkerTime = 3f;
    // blinker can turn off early, within reasonable max
    public float maxBlinkerOffTime;
    public GameObject bikeLane;

    public HeadCheck headCheckScript;
    // Turn must be made within reasonable time after head check
    public float maxHeadCheckDelay = 2f;

    private blinkers blinkerScript;
    private TextMeshProUGUI textElement;

    private int previousLane;

    void Start(){
        blinkerScript = motorbike.transform.Find("Dashboard Canvas/Blinkers").GetComponent<blinkers>();
        textElement = popup.transform.Find("Instructions").GetComponent<TextMeshProUGUI>();

        previousLane = -1;
    }

    public void enteredLane(GameObject lane) {
        int newLane = int.Parse(lane.name.Substring(Metrocycle.Constants.laneNamePrefix.Length));

        checkBlinkerForLaneChange(newLane);
        checkEnteredBikeLane(lane);
    }

    public void checkBlinkerForLaneChange(int newLane) {
        if (previousLane == -1) {
            // This is the first lane we entered so just record it and do nothing;
            previousLane = newLane;
            return;
        }

        if (newLane == previousLane) {
            return;
        }

        bool hasError = false;
        // HACK: For now, lets assume that the lanes on a road are numbered
        //       increasing from 0, left to Right
        Blinker which = (newLane > previousLane) ? Blinker.RIGHT : Blinker.LEFT;
        Debug.Log("Changed lane to the " + ((which == Blinker.LEFT) ? "Left" : "Right"));

        bool isBlinkerOn = ((which == Blinker.LEFT && blinkerScript.leftStatus == 1)
        || (which == Blinker.RIGHT && blinkerScript.rightStatus == 1));

        // HACK: only true when leftStatus == rightStatus == 0
        if (blinkerScript.leftStatus - blinkerScript.rightStatus == 0
            && Time.time - blinkerScript.blinkerOffTime <= maxBlinkerOffTime)
        {
            // blinker currently not on, but was on a few moments ago
            isBlinkerOn = which == blinkerScript.lastActiveBlinker;
        }

        if (!isBlinkerOn) {
            if (blinkerScript.leftStatus != blinkerScript.rightStatus) {
                textElement.text = "You used the blinker signalling the opposite direction!";
            } else {
                textElement.text = "You did not use your blinker lights before changing lanes.";
            }

            hasError = true;
        } else if (Time.time - blinkerScript.blinkerActivationTime < minBlinkerTime) {
            textElement.text = "You did not give ample time for other road users to react to your blinker signal.";
            hasError = true;
        }

        hasError = verifyHeadCheck(which) || hasError;

        previousLane = newLane;
        if (hasError) {
            popup.SetActive(true);
        } else {
            // Successful lane change, reset blinkerActivationTime
            // this is to prevent changing multiple lanes at once
            // HACK: modify property directly. Should use func/message
            blinkerScript.blinkerActivationTime = Time.time;
        }
    }

    public bool verifyHeadCheck(Blinker direction) {
        float turnTime = Time.time;
        float headCheckTime;
        if (direction == Blinker.LEFT) {
            headCheckTime = headCheckScript.leftCheckTime;
        } else {
            headCheckTime = headCheckScript.rightCheckTime;
        }

        float turnDelay = Time.time - headCheckTime;

        if (turnDelay > maxHeadCheckDelay) {
            textElement.text = "Make sure to perform a head check right before changing lanes.";
            popup.SetActive(true);
            return true;
        }

        if (headCheckTime < blinkerScript.blinkerActivationTime) {
            textElement.text = "Make sure to perform a head check even after you turn your blinker on.";
            popup.SetActive(true);
            return true;
        }

        return false;
    }

    public void checkEnteredBikeLane(GameObject lane) {
        if (lane != bikeLane) {
            return;
        }

        textElement.text = "Motorcycles are not allowed on the Bike Lane!";
        popup.SetActive(true);
    }
}
