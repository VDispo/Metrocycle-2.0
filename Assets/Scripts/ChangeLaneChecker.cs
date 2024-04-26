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

    private blinkers blinkerScript;

    private int previousLane;
    private string errorText = "";
    private string blinkerName;


    void Start(){
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();

        previousLane = -1;
        blinkerName = GameManager.Instance.blinkerName();
    }

    public void enteredLane(GameObject lane) {
        string lanePrefix = Metrocycle.Constants.laneNamePrefix;
        int lanePartStart = lane.name.LastIndexOf(lanePrefix) + lanePrefix.Length;
        int newLane = int.Parse(lane.name.Substring(lanePartStart));

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
        Direction which = (newLane > previousLane) ? Direction.RIGHT : Direction.LEFT;
        Debug.Log("Changed lane to the " + ((which == Direction.LEFT) ? "Left" : "Right"));

        bool isBlinkerOn = ((which == Direction.LEFT && blinkerScript.leftStatus == 1)
        || (which == Direction.RIGHT && blinkerScript.rightStatus == 1));

        // HACK: only true when leftStatus == rightStatus == 0
        if (blinkerScript.leftStatus - blinkerScript.rightStatus == 0
            && Time.time - blinkerScript.blinkerOffTime <= maxBlinkerOffTime)
        {
            // blinker currently not on, but was on a few moments ago
            isBlinkerOn = which == blinkerScript.lastActiveBlinker;
        }

        if (!isBlinkerOn) {
            if (blinkerScript.leftStatus != blinkerScript.rightStatus) {
                errorText = "You used the " + blinkerName + " for the opposite direction!";
            } else {
                errorText = "You did not use your" + blinkerName + " before changing lanes.";
            }

            hasError = true;
        } else if (Time.time - blinkerScript.blinkerActivationTime < minBlinkerTime) {
            errorText = "You did not give ample time for other road users to react to your " + blinkerName;
            hasError = true;
        }

        previousLane = newLane;
        if (hasError) {
            GameManager.Instance.PopupSystem.popError(
                "You changed lanes incorrectly", errorText
            );
        } else {
            GameManager.Instance.verifyHeadCheck(which);

            // Successful lane change, reset blinkerActivationTime
            // this is to prevent changing multiple lanes at once
            // HACK: modify property directly. Should use func/message
            blinkerScript.blinkerActivationTime = Time.time;
        }
    }

    public void checkEnteredBikeLane(GameObject lane) {
        if (GameManager.Instance.getBikeType() == Metrocycle.BikeType.Bicycle) {
            return;
        }

        if (lane != bikeLane) {
            return;
        }

        errorText = "Motorcycles are not allowed on the Bike Lane!";
        GameManager.Instance.PopupSystem.popError(
            "Uh oh!", errorText
        );
    }
}
