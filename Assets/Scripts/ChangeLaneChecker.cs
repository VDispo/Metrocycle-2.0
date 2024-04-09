using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeLaneChecker : MonoBehaviour
{
    public float minBlinkerTime = 3f;
    // blinker can turn off early, within reasonable max
    public float maxBlinkerOffTime;
    public GameObject bikeLane;

    public HeadCheck headCheckScript;
    // Turn must be made within reasonable time after head check
    public float maxHeadCheckDelay = 5f;

    private blinkers blinkerScript;

    private int previousLane;
    private string errorText = "";


    void Start(){
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();

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
                errorText = "You used the blinker signalling the opposite direction!";
            } else {
                errorText = "You did not use your blinker lights before changing lanes.";
            }

            hasError = true;
        } else if (Time.time - blinkerScript.blinkerActivationTime < minBlinkerTime) {
            errorText = "You did not give ample time for other road users to react to your blinker signal.";
            hasError = true;
        }

        hasError = verifyHeadCheck(which) || hasError;

        previousLane = newLane;
        if (hasError) {
            GameManager.Instance.PopupSystem.popError(
                "You changed lanes incorrectly", errorText
            );
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
        bool isDuringHeadCheck = false;
        if (direction == Blinker.LEFT) {
            headCheckTime = headCheckScript.leftCheckTime;
            isDuringHeadCheck = headCheckScript.isLookingLeft();
        } else {
            headCheckTime = headCheckScript.rightCheckTime;
            isDuringHeadCheck = headCheckScript.isLookingRight();
        }
        Debug.Log("Check" + headCheckScript.leftCheckTime + " " + headCheckScript.rightCheckTime  + " " + turnTime + " " + isDuringHeadCheck);

        float turnDelay = Time.time - headCheckTime;

        if (!isDuringHeadCheck) {
            if (turnDelay > maxHeadCheckDelay) {
                errorText = "Make sure to perform a head check right before changing lanes.";
                GameManager.Instance.PopupSystem.popError(
                    "Uh oh!", errorText
                );
                return true;
            }

            if (headCheckTime < blinkerScript.blinkerActivationTime) {
                errorText = "Make sure to perform a head check even after you turn your blinker on.";
                GameManager.Instance.PopupSystem.popError(
                    "Uh oh!", errorText
                );
                return true;
            }
        }

        return false;
    }

    public void checkEnteredBikeLane(GameObject lane) {
        if (lane != bikeLane) {
            return;
        }

        errorText = "Motorcycles are not allowed on the Bike Lane!";
        GameManager.Instance.PopupSystem.popError(
            "Uh oh!", errorText
        );
    }
}
