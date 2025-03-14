using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeLaneChecker : MonoBehaviour
{
    public GameObject bikeLane;
    public GameObject busLane;

    [SerializeField]
    public GameObject[] bicycleAllowedInLanes;
    public bool isBikeRoad;     // if true, bikes are allowed in all lanes
    private HashSet<GameObject> bicycleAllowed_Set = new HashSet<GameObject>();

    private blinkers blinkerScript;

    private int previousLane;
    private string errorText = "";
    private string blinkerName;

    private float lastDetectTime;

    public string PopupTitle { get; set; }
    public string BusLaneErrText { get; set; }
    public string BikeLaneErrText { get; set; }
    public string MotorLaneErrText { get; set; }


    void Start(){
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();

        previousLane = -1;
        blinkerName = GameManager.Instance.blinkerName();

        foreach (GameObject lane in bicycleAllowedInLanes) {
            bicycleAllowed_Set.Add(lane);
        }

        bicycleAllowed_Set.Add(bikeLane);
        lastDetectTime = -1;

        GameManager.Instance.resetSignal.AddListener(() => {
            previousLane = -1;
            lastDetectTime = -1;
        });
    }

    public void enteredLane(GameObject lane) {
        string lanePrefix = Metrocycle.Constants.laneNamePrefix;
        int lanePartStart = lane.name.LastIndexOf(lanePrefix) + lanePrefix.Length;
        int newLane = int.Parse(lane.name.Substring(lanePartStart));

        // NOTE: Problem: last remembered lane is "sticky"
        //  e.g. if we have two roads each with 2 lanes  ===(A) ====(B)
        //       if bike leaves road A at lane 1, drives through road B and changes to lane 2
        //       then in the perspective of road A, bike is still at lane 1
        // HACK: we make roads "forget" the last lane when enough time has passed (Set to 2s)
        //       assumption: current lane within road is updated regularly; this is true since
        //       we have evenly spaced lane detects of small enough size within roads (assuming use of MTS_ER3D automated waypoints)
        // TODO: use a more robust solution. E.g. every road tracks/listens on where the bike currently is. If the bike is outside this road, this road then forgets the previousLane
        if (lastDetectTime != -1 && Time.time - lastDetectTime > 10) {
            previousLane = -1;
        }
        lastDetectTime = Time.time;

        Debug.Log($"Entered Lane {lane}");
        bool hasError = checkEnteredBusOrBikeLane(lane);

        if (!hasError) hasError = checkBicycleEnteredForbiddenLane(lane);
        checkBlinkerForLaneChange(newLane);
    }

    public void checkBlinkerForLaneChange(int newLane) {
        if (previousLane == -1) {
            // This is the first lane we entered so just record it and do nothing;
            previousLane = newLane;
            Debug.Log($"ENTERED lane {newLane}");
            return;
        }

        if (newLane == previousLane) {
            return;
        }

        // HACK: For now, lets assume that the lanes on a road are numbered
        //       increasing from 0, leftmost to rightmost
        //       left lanes have even values, right lanes have odd values
        //       e.g. on a standard 4-lane, 2-way road
        //        <<< 2 <<<
        //        <<< 0 <<<
        //        >>> 1 >>>
        //        >>> 3 >>>
        Direction direction = (newLane > previousLane) ? Direction.RIGHT : Direction.LEFT;

        // changed parity => moved from right lane to left lane
        if (newLane % 2 != previousLane % 2) {
            direction = Direction.LEFT;
        }

        Debug.Log("Changed lane to the " + ((direction == Direction.LEFT) ? "Left" : "Right"));

        GameManager.Instance.checkProperTurnOrLaneChange(direction);

        previousLane = newLane;

        // Successful lane change, reset blinkerActivationTime
        // this is to prevent changing multiple lanes at once
        // HACK: modify property directly. Should use func/message
        blinkerScript.blinkerActivationTime = Time.time;
    }

    public bool checkEnteredBusOrBikeLane(GameObject lane) {
        if (lane == busLane) {
            errorText = BusLaneErrText;

            GameManager.Instance.setErrorReason(Metrocycle.ErrorReason.EXCLUSIVE_BUSLANE);
            return true;
        }

        if (GameManager.Instance.getBikeType() == Metrocycle.BikeType.Bicycle) {
            return false;
        }

        if (lane == bikeLane) {
            errorText = BikeLaneErrText;

            GameManager.Instance.setErrorReason(Metrocycle.ErrorReason.EXCLUSIVE_BIKELANE);
        }  else {
            return false;
        }

        GameManager.Instance.PopupSystem.popError(
            PopupTitle, errorText
        );

        return true;
    }

    public bool checkBicycleEnteredForbiddenLane(GameObject lane) {
        if (GameManager.Instance.getBikeType() != Metrocycle.BikeType.Bicycle
            || isBikeRoad
        ) {
            return false;
        }

        if (!bicycleAllowed_Set.Contains(lane)) {
            errorText = MotorLaneErrText;
            GameManager.Instance.PopupSystem.popError(
                PopupTitle, errorText
            );

            GameManager.Instance.setErrorReason(Metrocycle.ErrorReason.BIKE_NOTALLOWED);
            return true;
        }

        return false;
    }

    public void addBikeAllowedLane(GameObject lane) {
        bicycleAllowed_Set.Add(lane);
    }
}
