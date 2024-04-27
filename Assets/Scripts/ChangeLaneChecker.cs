using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeLaneChecker : MonoBehaviour
{
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
        Direction direction = (newLane > previousLane) ? Direction.RIGHT : Direction.LEFT;
        Debug.Log("Changed lane to the " + ((direction == Direction.LEFT) ? "Left" : "Right"));

        GameManager.Instance.checkProperTurnOrLaneChange(direction);

        previousLane = newLane;
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
