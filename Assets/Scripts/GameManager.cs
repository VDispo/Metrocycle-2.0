using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

/* To use, attach this script to the "Bike" prefab instance
 */

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public popUp PopupSystem = null;
    public HeadCheck HeadCheckScript = null;

    public bool isAndroid; // FOR TESTING ANDROID CONTROLS

    public GameObject mobileControlsCanvas;

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

    public Slider progressBar;
    public Text scoreText;
    public Text deductReasonText;

    public bool isTestMode = false;
    public List<string> userErrors;
    public bool isTutorialMode = false;
    public TutorialProgression tutorialManager = null;

    private int userScore = 100;

    private void Awake()
    {
        isAndroid = Application.platform == RuntimePlatform.Android;
        isAndroid = true; // For Android Build
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
        if (isAndroid) {
            mobileControlsCanvas.SetActive(true);
        } else {
            mobileControlsCanvas.SetActive(false);
        }

        // setBikeType(Metrocycle.BikeType.Motorcycle);    // TODO: move this call to selection of motorcycle or bicycle
        bikeTransform = gameObject.transform.GetChild(2).gameObject;
    }

    public GameObject setBikeType(Metrocycle.BikeType type)
    {
        // disable previous bike
        if (bike != null) {
            Debug.LogWarning("WARNING: Bike type can only be set once per Scene");
            Debug.LogWarning("WARNING: Calling setBikeType() twice results to blinkers not working");
            // return bike;
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
        // NOTE: we disable pausing in auto tests since 0 timescale also disables progress in tests
        if (!isTestMode) {
            Time.timeScale = 0;
        }
        mobileControlsCanvas.SetActive(false);
    }
    public void resumeGame() {
        setDashboardVisibility(true);
        Time.timeScale = 1;
        mobileControlsCanvas.SetActive(true);
    }
    public void restartGame() {
        setDashboardVisibility(true);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        mobileControlsCanvas.SetActive(true);
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

    public bool verifyHeadCheck(Direction direction, float turnTime=Metrocycle.Constants.ASSUME_HEADCHECK) {
        if (Mathf.Abs(turnTime - (Metrocycle.Constants.ASSUME_HEADCHECK)) < 0.1f) {
            Debug.Log("ASSUMING Head check was done. Set time to now so that checks succeed.");
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
        Debug.Log($"HEAD Check {isDuringHeadCheck}" + HeadCheckScript.leftCheckTime + " " + HeadCheckScript.rightCheckTime  + $" {headCheckTime} TURN" + turnTime + $" CurTime: {Time.time} blinker {blinkerScript.blinkerActivationTime}");

        if (isDuringHeadCheck) {
            return true;
        }


        if (headCheckTime != -1 && headCheckTime < blinkerScript.blinkerActivationTime) {
            string errorTitle = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "headCheckTitle");
            string errorText = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "headCheckAfterBlinker");
            Instance.setErrorReason(Metrocycle.ErrorReason.NO_HEADCHECK_AFTER_BLINKER);
            Instance.addUserError();

            Instance.PopupSystem.popPrompt(
                errorTitle, string.Format($"{errorText}", Instance.blinkerName()), true
            );

            return false;
        }

        float turnDelay = turnTime - headCheckTime;
        if (turnDelay > HeadCheckScript.maxHeadCheckDelay) {
            string errorTitle = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "headCheckTitle");
            string errorText = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "headCheckBeforeTurn");

            if (turnDelay > 3*HeadCheckScript.maxHeadCheckDelay || headCheckTime == -1) {
                // last headcheck was very long ago, driver probably forget to do head check at all
                Instance.setErrorReason(
                    direction == Direction.LEFT
                    ? Metrocycle.ErrorReason.LEFTTURN_NO_HEADCHECK
                    : Metrocycle.ErrorReason.RIGHTTURN_NO_HEADCHECK
                );
            } else {
                // last headcheck reasonably "recent", but not recent enough to be valid
                Instance.setErrorReason(Metrocycle.ErrorReason.EXPIRED_HEADCHECK);
            }
            Instance.addUserError();

            Instance.PopupSystem.popPrompt(
                errorTitle, errorText, true
            );



            return false;
        }

        updateUserScoreIncrementReason(5, "Head Check Bonus");
        // FALLTRHOUGH: no error found
        return true;
    }

    public void checkProperTurnOrLaneChange(Direction direction, float headCheckRefTime=Metrocycle.Constants.ASSUME_HEADCHECK, bool requireHeadCheck=true) {
        // NOTE: headCheckRefTime if the time when head check should have been checked
        // e.g.  when performing a U-turn, checkProperTurnOrLaneChange can only be
        //       called AFTER the U-turn is complete (i.e. at exit instead of at entry)
        //       but head check should have been called at ENTRY time

        bool isBlinkerOn = ((direction == Direction.LEFT && blinkerScript.leftStatus == 1)
        || (direction == Direction.RIGHT && blinkerScript.rightStatus == 1));

        // HACK: only true when leftStatus == rightStatus == 0
        if (blinkerScript.leftStatus + blinkerScript.rightStatus == 0
            && Time.time - blinkerScript.blinkerOffTime <= blinkerScript.maxBlinkerOffTime
            && blinkerScript.blinkerOffTime != -1)
        {
            // blinker currently not on, but was on a few moments ago
            isBlinkerOn = direction == blinkerScript.lastActiveBlinker;
        }

        string blinkerName = GameManager.Instance.blinkerName();
        string errorTitle = "";
        string errorText = "";
        bool hasError = false;
        if (!isBlinkerOn) {
            errorTitle = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "blinkerErrorTitle"); 
            Debug.Log($"GETTING BLINKER ERROR {errorTitle}");
            if (blinkerScript.leftStatus != blinkerScript.rightStatus)
            {
                errorText = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "wrongBlinkerDescription");

                Instance.setErrorReason(Metrocycle.ErrorReason.WRONG_BLINKER);
            }
            else
            {
                errorText = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "noBlinkerDescription");

                Instance.setErrorReason(
                    direction == Direction.LEFT
                    ? Metrocycle.ErrorReason.LEFTTURN_NO_BLINKER
                    : Metrocycle.ErrorReason.RIGHTTURN_NO_BLINKER
                );
            }

            hasError = true;
        } else if (Time.time - blinkerScript.blinkerActivationTime < blinkerScript.minBlinkerTime) {
            errorTitle = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "tooLongToTurnTitle");
            errorText = LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "tooLongToTurnDescription");
            hasError = true;

            Instance.setErrorReason(Metrocycle.ErrorReason.SHORT_BLINKER_TIME);
        } 

        if (hasError) {
            string text = string.Format($"{errorText}", blinkerName);
            Debug.Log($"BLINKER TITLE ERROR {errorTitle}");

            Debug.Log($"BLINKER ERROR {text}");
            Instance.PopupSystem.popPrompt(
                errorTitle, text, true
            );
            Instance.addUserError();

        } else {
            if (requireHeadCheck) {
                Instance.verifyHeadCheck(direction, headCheckRefTime);
            } else {
                updateUserScoreIncrementReason(5, "Blinker Bonus");
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

    public void setBikeSpeed(Vector3 speed)
    {
        stopBike();
        bikeRB.velocity = (speed/speed.magnitude) * (speed.magnitude / 3);
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
        GameManager.Instance.pauseGame();

        // Clear traffic in teleport location to prevent collision on spawn
        // And also clear up jams near save points
        // NOTE: radius of 100 is hardcoded for now
        // NOTE: Disabled for now since the assembly hierarchy for Gley will become cyclical
        // TODO: re-enable this and fix assembly hierarchy
        // GleyTrafficSystem.Manager.ClearTrafficOnArea(saveStateDetect.transform.position, 100);
        GameManager.Instance.teleportBike(saveStateDetect.transform);

        if (detect?.loadStateCallback != null) {
            detect.loadStateCallback.Invoke();
        }

        // inform listeners we loaded save state so they should probably reset states (e.g. current lane, intersection entry)
        resetSignal.Invoke();

        // Resume
        GameManager.Instance.resumeGame();
    }
    public bool hasSaveState()
    {
        return saveStateDetect != null;
    }

    public Metrocycle.ErrorReason getLastErrorReason()
    {
        return lastErrorReason;
    }
    public void setErrorReason(Metrocycle.ErrorReason er)
    {
        lastErrorReason = er;
    }
    public void resetErrorReason()
    {
        lastErrorReason = Metrocycle.ErrorReason.NOERROR;
    }

    public void updateProgressBar(int current_value, int max_value) 
    {
        progressBar.value = (float)current_value / max_value;
        if (isTutorialMode) {
            tutorialManager.tutorialProgress(current_value);
        }
    }

    public void addUserError()
    {
        userErrors.Add(lastErrorReason.ToString());
        updateUserScoreErrorReason();
    }
    public void updateUserScoreIncrementReason(int increment, string reason)
    {
        updateUserScoreIncrement(increment);
        setIncrementReasonText($"+{increment} {reason}");
    }

    public void updateUserScoreErrorReason()
    {
        Dictionary<Metrocycle.ErrorReason, int> errorScoreDeduction = new Dictionary<Metrocycle.ErrorReason, int>
        {
            { Metrocycle.ErrorReason.NO_HEADCHECK_AFTER_BLINKER, 15 },
            { Metrocycle.ErrorReason.LEFTTURN_NO_HEADCHECK, 15 },
            { Metrocycle.ErrorReason.RIGHTTURN_NO_HEADCHECK, 15 },
            { Metrocycle.ErrorReason.EXPIRED_HEADCHECK, 10 },
            { Metrocycle.ErrorReason.LEFTTURN_NO_BLINKER, 30 },
            { Metrocycle.ErrorReason.RIGHTTURN_NO_BLINKER, 30 },
            { Metrocycle.ErrorReason.WRONG_BLINKER, 50 },
            { Metrocycle.ErrorReason.SHORT_BLINKER_TIME, 15 }
        };

        Dictionary<Metrocycle.ErrorReason, string> errorDescriptions = new Dictionary<Metrocycle.ErrorReason, string>
        {
            { Metrocycle.ErrorReason.NO_HEADCHECK_AFTER_BLINKER, "No Head Check" },
            { Metrocycle.ErrorReason.LEFTTURN_NO_HEADCHECK, "No Left Head Check" },
            { Metrocycle.ErrorReason.RIGHTTURN_NO_HEADCHECK, "No Right Head Check" },
            { Metrocycle.ErrorReason.EXPIRED_HEADCHECK, "Expired Head Check" },
            { Metrocycle.ErrorReason.LEFTTURN_NO_BLINKER, "No Left Blinker" },
            { Metrocycle.ErrorReason.RIGHTTURN_NO_BLINKER, "No Right Blinker" },
            { Metrocycle.ErrorReason.WRONG_BLINKER, "Wrong Blinker" },
            { Metrocycle.ErrorReason.SHORT_BLINKER_TIME, "Short Blinker Time" }
        };

        Dictionary<Metrocycle.ErrorReason, string> mistakeTitles = new Dictionary<Metrocycle.ErrorReason, string>
        {
            { Metrocycle.ErrorReason.NO_HEADCHECK_AFTER_BLINKER, "No Head Check Performed" },
            { Metrocycle.ErrorReason.LEFTTURN_NO_HEADCHECK, "No Head Check Performed" },
            { Metrocycle.ErrorReason.RIGHTTURN_NO_HEADCHECK, "No Head Check Performed" },
            { Metrocycle.ErrorReason.EXPIRED_HEADCHECK, "Head Check Expired" },
            { Metrocycle.ErrorReason.LEFTTURN_NO_BLINKER, "No Blinker Used" },
            { Metrocycle.ErrorReason.RIGHTTURN_NO_BLINKER, "No Blinker Used" },
            { Metrocycle.ErrorReason.WRONG_BLINKER, "Wrong Blinker Direction" },
            { Metrocycle.ErrorReason.SHORT_BLINKER_TIME, "Blinker Time Too Short" }
        };

        if (errorScoreDeduction.TryGetValue(lastErrorReason, out int deduction))
        {
            updateUserScoreDeduction(deduction);
            string errorDescription = errorDescriptions[lastErrorReason];
            setDeductReasonText($"-{deduction} {errorDescription}");
            string mistakeTitle = mistakeTitles[lastErrorReason];
            Instance.PopupSystem.addUserError(mistakeTitle);
        }
        else
        {
            Debug.LogError("Error reason not found in dictionary: " + lastErrorReason);
        }
        
    }

    public void updateUserScoreDeduction(int deduction)
    {
        userScore -= deduction;
        if (userScore < 0) {
            PopupSystem.popError(
                LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "userMistakeLimitTitle"),
                LocalizationCache.Instance.GetLocalizedString("GenericPromptsTable", "userMistakeLimitDescription")
            );
        }
        scoreText.text = userScore.ToString();
    }

    public void updateUserScoreIncrement(int increment)
    {
        userScore += increment;
        if (userScore > 100) {
            userScore = 100;
        }
        scoreText.text = userScore.ToString();
    }

    public void setDeductReasonText(string text)
    {
        deductReasonText.GetComponent<DeductionTextBehavior>().SetErrorText(text);
    }
    public void setIncrementReasonText(string text)
    {
        deductReasonText.GetComponent<DeductionTextBehavior>().SetIncrementText(text);
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
