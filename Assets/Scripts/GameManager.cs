using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/* To use, attach this script to the "Bike" prefab instance
 */

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public popUp PopupSystem = null;
    public HeadCheck HeadCheckScript = null;

    public GameObject bike;
    public UnityEvent resetSignal;

    private Metrocycle.BikeType bikeType;
    private Rigidbody bikeRB;

    private GameObject bikeTransform;

    private blinkers blinkerScript;

    private GameObject saveStateDetect = null;

    // TODO: Maybe centralize all error messages in one location, just decide based on lastErrorReason?
    //       This will make it easier to see ALL checks and make translation of error messages easier
    private Metrocycle.ErrorReason lastErrorReason = Metrocycle.ErrorReason.NOERROR;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (PopupSystem == null) {
            PopupSystem = GameObject.Find("/popUp").GetComponent<popUp>();
            Debug.Log("PopupSystem Force Initialized " + PopupSystem);
        }
        if (HeadCheckScript == null) {
            HeadCheckScript = GameObject.Find("/Cameras/Main Camera").GetComponent<HeadCheck>();
            Debug.Log("HeadCheckScript Force Initialized " + HeadCheckScript);
        }

        // setBikeType(Metrocycle.BikeType.Motorcycle);    // TODO: move this call to selection of motorcycle or bicycle
        bikeTransform = gameObject.transform.GetChild(2).gameObject;
    }

    public GameObject setBikeType(Metrocycle.BikeType type)
    {
        // disable previous bike
        if (bike != null) {
            Debug.LogError("Bicycle type can only be set once per Scene");
            return bike;
        }

        switch(type) {
            case Metrocycle.BikeType.Bicycle:
                bike = gameObject.transform.GetChild(1).gameObject;
                Debug.Assert(bike.name == "Bicycle");
                break;
            default:
            case Metrocycle.BikeType.Motorcycle:
                bike = gameObject.transform.GetChild(0).gameObject;
                Debug.Assert(bike.name == "Motorcycle");
                break;
        }

        bikeType = type;

        bikeRB = bike.GetComponent<Rigidbody>();
        bike.AddComponent<CollisionWithObstacles>();
        blinkerScript = GameManager.Instance.getBlinkers().GetComponent<blinkers>();
        blinkerScript.setBikeType(type);

        bike.SetActive(true);
        return bike;
    }

    public Metrocycle.BikeType getBikeType()
    {
        return bikeType;
    }

    public float getBikeSpeed()
    {
        return bikeRB.velocity.magnitude * 3;     // HACK: *3 is just based on "feel" for now
    }

    public GameObject getDashboard()
    {
        GameObject dashboardCanvas = gameObject.transform.GetChild(3).gameObject;
        Debug.Assert(dashboardCanvas.name == "Dashboard Canvas");

        return dashboardCanvas;
    }
    public GameObject getBlinkers()
    {
        GameObject blinkers = getDashboard().transform.GetChild(1).gameObject;
        Debug.Assert(blinkers.name == "Blinkers");

        return blinkers;
    }

    public void pauseGame() {
        setDashboardVisibility(false);
        Time.timeScale = 0;
    }
    public void resumeGame() {
        setDashboardVisibility(true);
        Time.timeScale = 1;
    }
    public void restartGame() {
        setDashboardVisibility(true);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void setDashboardVisibility(bool isVisible) {
        GameObject dashboard = getDashboard();
        // NOTE: hardcoded to prefab: Only first two children (Speedometer, Blinkers) are actually part of dashboard
        //       Timer and Pause buttons are just part of UI (should not be hidden on prompt)
        foreach (int i in new int[]{0, 1}) {
            dashboard.transform.GetChild(i).gameObject.SetActive(isVisible);
        }
    }

    public string blinkerName()
    {
        if (bikeType == Metrocycle.BikeType.Motorcycle) {
            return "blinker";
        } else {
            return "hand signals";
        }
    }

    public bool isDoingHeadCheck(Direction direction)
    {
        bool isValid = false;
        switch (direction) {
            case Direction.LEFT:
                isValid = HeadCheckScript.isLookingLeft();
                break;
            case Direction.RIGHT:
                isValid = HeadCheckScript.isLookingRight();
                break;
            case Direction.FORWARD:
                isValid = HeadCheckScript.isLookingForward();
                break;
            default:
                Debug.LogError("Invalid Direction " + direction);
                break;
        }

        return isValid;
    }

    public bool verifyHeadCheck(Direction direction, float turnTime=-1f) {
        if (Mathf.Abs(turnTime - (-1f)) < 0.1f) {
            turnTime = Time.time;
        }

        // Debug.Log("Turn Time " + turnTime + " curTime " + Time.time);

        float headCheckTime;

        Debug.Assert(direction != Direction.FORWARD);

        if (direction == Direction.LEFT) {
            headCheckTime = HeadCheckScript.leftCheckTime;
        } else {
            headCheckTime = HeadCheckScript.rightCheckTime;
        }

        bool isDuringHeadCheck = isDoingHeadCheck(direction);
        Debug.Log("Check" + HeadCheckScript.leftCheckTime + " " + HeadCheckScript.rightCheckTime  + " " + turnTime + " " + isDuringHeadCheck);

        if (isDuringHeadCheck) {
            return true;
        }

        float turnDelay = turnTime - headCheckTime;
        if (turnDelay > HeadCheckScript.maxHeadCheckDelay) {
            const string errorText = "Make sure to perform a head check right before changing lanes or turning.";
            GameManager.setErrorReason(Metrocycle.ErrorReason.EXPIRED_HEADCHECK);

            GameManager.Instance.PopupSystem.popError(
                "Uh oh!", errorText
            );

            return false;
        }

        if (headCheckTime < blinkerScript.blinkerActivationTime) {
            string errorText = "Make sure to perform a head check even after you use your " + GameManager.Instance.blinkerName();
            GameManager.setErrorReason(Metrocycle.ErrorReason.NO_HEADCHECK_AFTER_BLINKER);

            GameManager.Instance.PopupSystem.popError(
                "Uh oh!", errorText
            );

            return false;
        }

        // FALLTRHOUGH: no error found
        return true;
    }

    public void checkProperTurnOrLaneChange(Direction direction, float headCheckRefTime=-1f, bool requireHeadCheck=true) {
        // NOTE: headCheckRefTime if the time when head check should have been checked
        // e.g.  when performing a U-turn, checkProperTurnOrLaneChange can only be
        //       called AFTER the U-turn is complete (i.e. at exit instead of at entry)
        //       but head check should have been called at ENTRY time

        bool isBlinkerOn = ((direction == Direction.LEFT && blinkerScript.leftStatus == 1)
        || (direction == Direction.RIGHT && blinkerScript.rightStatus == 1));

        // HACK: only true when leftStatus == rightStatus == 0
        if (blinkerScript.leftStatus + blinkerScript.rightStatus == 0
            && Time.time - blinkerScript.blinkerOffTime <= blinkerScript.maxBlinkerOffTime)
        {
            // blinker currently not on, but was on a few moments ago
            isBlinkerOn = direction == blinkerScript.lastActiveBlinker;
        }

        string blinkerName = GameManager.Instance.blinkerName();
        string errorText = "";
        bool hasError = false;
        if (!isBlinkerOn) {
            if (blinkerScript.leftStatus != blinkerScript.rightStatus) {
                errorText = "You used the " + blinkerName + " for the opposite direction!";

                GameManager.setErrorReason(Metrocycle.ErrorReason.WRONG_BLINKER);
            } else {
                errorText = "You did not use your " + blinkerName + " before changing lanes or turning.";

                GameManager.setErrorReason(
                    direction == Direction.LEFT
                    ? Metrocycle.ErrorReason.LEFTTURN_NO_BLINKER
                    : Metrocycle.ErrorReason.RIGHTTURN_NO_BLINKER
                );
            }

            hasError = true;
        } else if (Time.time - blinkerScript.blinkerActivationTime < blinkerScript.minBlinkerTime) {
            errorText = "You did not give ample time for other road users to react to your " + blinkerName + ".\nIt is recommended to indicate your intent 5s before the action (e.g. lane change).";
            hasError = true;

            GameManager.setErrorReason(Metrocycle.ErrorReason.SHORT_BLINKER_TIME);
        }

        if (hasError) {
            GameManager.Instance.PopupSystem.popError(
                "Uh oh", errorText
            );
        } else {
            if (requireHeadCheck) {
                GameManager.Instance.verifyHeadCheck(direction, headCheckRefTime);
            }
        }
    }

    public void startBlinkerCancelTimer()
    {
        blinkerScript.startBlinkerCancelTimer();
    }

    public void teleportBike(Transform newTransform)
    {
        bike.SetActive(false);
        gameObject.transform.position = newTransform.position;
        gameObject.transform.rotation = newTransform.rotation;

        bike.transform.position = newTransform.position;
        bike.transform.rotation = newTransform.rotation;

        // Kill velocity, we don't want bike to move after teleport
        bikeRB.velocity = new Vector3(0, 0, 0);

        bike.SetActive(true);
        Debug.Log("Bike teleported to " + newTransform);
    }

    public void stopBike()
    {
        bikeRB.velocity = new Vector3(0, 0, 0);
    }

    public void setSaveState(CheckpointDetection detect)
    {
        saveStateDetect = detect.gameObject;
        Debug.Log("SAVE STATE " + detect.gameObject);
        PopupSystem?.setErrorBehavior(popUp.ErrorBehavior.LoadSave);
    }
    public void loadSaveState()
    {
        CheckpointDetection detect = saveStateDetect.GetComponent<CheckpointDetection>();
        // HACK: always teleport bike to location of save Detect
        // TODO: make more generic (teleport location optional/can be supplied idependently)
        // Pause
        Debug.Log("LOADING SAVE " + detect);
        Time.timeScale = 0;

        // Clear traffic in teleport location to prevent collision on spawn
        // And also clear up jams near save points
        // NOTE: radius of 100 is hardcoded for now
        GleyTrafficSystem.Manager.ClearTrafficOnArea(saveStateDetect.transform.position, 100);
        GameManager.Instance.teleportBike(saveStateDetect.transform);

        if (detect?.loadStateCallback != null) {
            detect.loadStateCallback.Invoke();
        }

        // inform listeners we loaded save state so they should probably reset states (e.g. current lane, intersection entry)
        resetSignal.Invoke();

        // Resume
        Time.timeScale = 1;
    }
    public bool hasSaveState()
    {
        return saveStateDetect != null;
    }

    public void getLastErrorReason(Metrocycle.ErrorReason er)
    {
        return lastErrorReason;
    }
    public void setErrorReason(Metrocycle.ErrorReason er)
    {
        lastErrorReason = er;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PopupSystem.popPause();
        }

        if (bike == null) {
            return;
        }

        bikeTransform.transform.position = bike.transform.position;
        bikeTransform.transform.rotation = bike.transform.rotation;
        bikeTransform.transform.localScale = bike.transform.localScale;
    }
}
