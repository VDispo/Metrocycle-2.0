using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* To use, attach this script to the "Bike" prefab instance
 */

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public popUp PopupSystem = null;
    public HeadCheck HeadCheckScript = null;

    public GameObject bike;
    private Metrocycle.BikeType bikeType;
    private Rigidbody bikeRB;

    private GameObject bikeTransform;

    private blinkers blinkerScript;


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
        getDashboard().SetActive(false);
        Time.timeScale = 0;
    }
    public void resumeGame() {
        getDashboard().SetActive(true);
        Time.timeScale = 1;
    }
    public void restartGame() {
        getDashboard().SetActive(true);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public bool verifyHeadCheck(Direction direction) {
        float turnTime = Time.time;
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

        float turnDelay = Time.time - headCheckTime;
        if (turnDelay > HeadCheckScript.maxHeadCheckDelay) {
            const string errorText = "Make sure to perform a head check right before changing lanes or turning.";
            GameManager.Instance.PopupSystem.popError(
                "Uh oh!", errorText
            );

            return false;
        }

        if (headCheckTime < blinkerScript.blinkerActivationTime) {
            string errorText = "Make sure to perform a head check even after you use your " + GameManager.Instance.blinkerName();
            GameManager.Instance.PopupSystem.popError(
                "Uh oh!", errorText
            );

            return false;
        }

        // FALLTRHOUGH: no error found
        return true;
    }

    public void checkProperTurnOrLaneChange(Direction direction, bool requireHeadCheck=true) {
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
            } else {
                errorText = "You did not use your " + blinkerName + " before changing lanes or turning.";
            }

            hasError = true;
        } else if (Time.time - blinkerScript.blinkerActivationTime < blinkerScript.minBlinkerTime) {
            errorText = "You did not give ample time for other road users to react to your " + blinkerName;
            hasError = true;
        }

        if (hasError) {
            GameManager.Instance.PopupSystem.popError(
                "Uh oh", errorText
            );
        } else {
            if (requireHeadCheck) {
                GameManager.Instance.verifyHeadCheck(direction);
            }
        }
    }

    public void startBlinkerCancelTimer()
    {
        blinkerScript.startBlinkerCancelTimer();
    }

    void Update() {
        if (bike == null) {
            return;
        }

        bikeTransform.transform.position = bike.transform.position;
        bikeTransform.transform.rotation = bike.transform.rotation;
        bikeTransform.transform.localScale = bike.transform.localScale;
    }
}
