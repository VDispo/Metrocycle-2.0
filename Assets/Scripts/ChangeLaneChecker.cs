using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeLaneChecker : MonoBehaviour
{
    public GameObject bikeLane;

    [SerializeField]
    public GameObject[] bicycleAllowedInLanes;
    private HashSet<GameObject> bicycleAllowed_Set;

    private blinkers blinkerScript;

    private int previousLane;
    private string errorText = "";
    private string blinkerName;


    void Start(){
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();

        previousLane = -1;
        blinkerName = GameManager.Instance.blinkerName();

        bicycleAllowed_Set = new HashSet<GameObject>(bicycleAllowedInLanes);
        bicycleAllowed_Set.Add(bikeLane);
    }

    public void enteredLane(GameObject lane) {
        string lanePrefix = Metrocycle.Constants.laneNamePrefix;
        int lanePartStart = lane.name.LastIndexOf(lanePrefix) + lanePrefix.Length;
        int newLane = int.Parse(lane.name.Substring(lanePartStart));

        checkEnteredBikeLane(lane);
        checkBicycleEnteredForbiddenLane(lane);
        checkBlinkerForLaneChange(newLane);
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

        // HACK: For now, lets assume that the lanes on a road are numbered
        //       increasing from 0, left to Right
        Direction direction = (newLane > previousLane) ? Direction.RIGHT : Direction.LEFT;
        Debug.Log("Changed lane to the " + ((direction == Direction.LEFT) ? "Left" : "Right"));

        GameManager.Instance.checkProperTurnOrLaneChange(direction);

        previousLane = newLane;

        // Successful lane change, reset blinkerActivationTime
        // this is to prevent changing multiple lanes at once
        // HACK: modify property directly. Should use func/message
        blinkerScript.blinkerActivationTime = Time.time;
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

    public void checkBicycleEnteredForbiddenLane(GameObject lane) {
        if (GameManager.Instance.getBikeType() != Metrocycle.BikeType.Bicycle) {
            return;
        }

        if (!bicycleAllowed_Set.Contains(lane)) {
            errorText = "Bicycles are not allowed in this lane which is used by motored vehicles.";
            GameManager.Instance.PopupSystem.popError(
                "Uh oh!", errorText
            );
        }
    }
}
