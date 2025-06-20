using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum PopupType {
    START,
    PROMPT,
    ERROR,
    // NOTE: These types don't take title/text args
    PAUSE,
    FINISH,
    // NOTE: these are from IntersectionChecker.cs
    // TODO: should be properly consolidated
    WARNING,    // weaker error (not forced to restart)
    INFO        // just pure information; synonym for prompt
};


public class popUp : MonoBehaviour
{
    public enum ErrorBehavior {
        Reset,
        LoadSave
    };

    public GameObject popUpSystem;
    public GameObject throttleUI;
    public GameObject headCheckUI;

    private Transform lastActiveSet = null;
    private Transform popUpBox;

    delegate void PopDelegate(string header, string body, bool countAsError);
    void Awake()
    {
        popUpBox = popUpSystem.transform.Find("popUpBox");
        for (int i = 0; i < transform.childCount; ++i) {
            Transform popupSet = transform.GetChild(i);
            // Assume all buttons are direct children of a set
            for (int j = 0; j < popupSet.childCount; ++j) {
                Transform button = popupSet.GetChild(j);
                Button buttonScript = button.GetComponent<Button>();
                if (buttonScript == null) {
                    continue;
                }

                // HACK: Just use name to determine function
                if (button.name.StartsWith("start")
                    || button.name.StartsWith("continue")
                ) {
                    buttonScript.onClick.AddListener(() => {
                        closePopup(); GameManager.Instance.resumeGame();
                        GameManager.Instance.setDashboardVisibility(true);
                    });
                } else if (button.name.StartsWith("reset")) {
                    buttonScript.onClick.AddListener(() => {
                        closePopup(); GameManager.Instance.restartGame();
                    });
                } else if (button.name.StartsWith("menu")) {
                    buttonScript.onClick.AddListener(() => {
                        closePopup(); CustomSceneManager.SwitchScene("Start Screen");
                    });
                } else if (button.name.StartsWith("loadSaveState")) {
                    buttonScript.onClick.AddListener(() => {
                        if (GameManager.Instance.hasSaveState()) {
                            GameManager.Instance.loadSaveState();
                        } else {
                            GameManager.Instance.restartGame();
                        }
                        closePopup();
                    });
                }

            }
        }
    }

    public void popWithType(PopupType type, string headerMessage, string bodyMessage, bool countAsError=false)
    {
        switch(type) {
            case PopupType.PAUSE:
                popPause();
                return;
            case PopupType.FINISH:
                popFinish();
                return;
        }

        PopDelegate popFunc = null;
        switch(type) {
            case PopupType.START:
                popFunc = popStart;
                break;
            case PopupType.ERROR:
                popFunc = popError;
                countAsError = true;
                break;
            case PopupType.PROMPT:
                popFunc = popPrompt;
                break;
            default:
                Debug.LogError("popUp: Only START, ERROR, PROMPT supported for now");
                return;
        }

        if (popFunc != null) {
            popFunc(headerMessage, bodyMessage, countAsError);
        } else {

        }
    }

    public void popStart(string headerMessage, string bodyMessage, bool countAsError=false)
    {
        // NOTE: countAsError parameter is ignored; only added to keep function signature of pop* functions consistent

        Transform startSet = popUpSystem.transform.Find("startSet");

        setHeaderText(startSet, headerMessage);
        setBodyText(startSet, bodyMessage);

        GameManager.Instance.setDashboardVisibility(false);
        Stats.ResetStats();
        showPopup(startSet);
    }

    public void popPause()
    {
        Transform pauseSet = popUpSystem.transform.Find("pauseSet");
        showPopup(pauseSet);
    }

    public void popPrompt(string headerMessage, string bodyMessage, bool countAsError=false)
    {
        Transform promptSet = popUpSystem.transform.Find("promptSet");

        setHeaderText(promptSet, headerMessage);
        setBodyText(promptSet, bodyMessage);

        if (countAsError) {
            Stats.incrementErrors();
        }

        showPopup(promptSet);
    }

    public void popError(string headerMessage, string bodyMessage, bool countAsError=true)
    {
        // NOTE: countAsError parameter is ignored; only added to keep function signature of pop* functions consistent

        Transform errorSet = popUpSystem.transform.Find("errorSet");

        Stats.incrementErrors();
        setHeaderText(errorSet, headerMessage);
        setBodyText(errorSet, bodyMessage);

        showPopup(errorSet);
    }

    public void popFinish()
    {
        Stats.SetSpeed();
        Stats.SetTime();
        int score = GameManager.Instance.getUserScore();

        Debug.Log($"User score: {score}");

        (float speed, float elapsedTime, int errors, string[] errorsClassification) = Stats.GetStats();
        string sceneName = SceneManager.GetActiveScene().name;
        #if !UNITY_EDITOR && UNITY_WEBGL
            Stats.SaveStats_JS(sceneName, speed, elapsedTime, errors, errorsClassification, score);
        #else
            Stats.SaveStats(sceneName, speed, elapsedTime, errors, errorsClassification, score);
        #endif
        // Get the current scene name and parse vehicle type and scenario
        string[] sceneParts = sceneName.Split('_');
        Debug.Log("SCENE PARTS" + sceneParts.Length);
        string scenarioType = sceneParts.Length > 0 ? sceneParts[0] : "";
        string vehicleType = sceneParts.Length == 2 ? sceneParts[1] : sceneParts[2];

        if (scenarioType == "Tutorial")
        {
            PlayerPrefs.SetInt($"{vehicleType}_TutorialCompleted", 1);
        }
        else if (scenarioType == "Basic")
        {
            PlayerPrefs.SetInt($"{vehicleType}_Basic_{sceneParts[1]}", 1);
            int straight = PlayerPrefs.GetInt($"{vehicleType}_Basic_Straight", 0);
            int uturn = PlayerPrefs.GetInt($"{vehicleType}_Basic_Uturn", 0);
            int intersection = PlayerPrefs.GetInt($"{vehicleType}_Basic_Intersection", 0);
            int multilane = PlayerPrefs.GetInt($"{vehicleType}_Basic_Multilane", 0);
            PlayerPrefs.SetInt($"{vehicleType}_BasicCompleted", straight + uturn + intersection + multilane);

            Debug.Log($"Basic {vehicleType} completed: {PlayerPrefs.GetInt($"{vehicleType}_BasicCompleted", 0)}");
        }

        Debug.Log($"Scenario Type: {scenarioType}, Vehicle Type: {vehicleType}");
        Transform finishSet = popUpSystem.transform.Find("finishSet");

        setFinishText(finishSet, speed, elapsedTime, errors, errorsClassification, score);

        if (scenarioType != "Tutorial")
        {
            Stats.UpdateLeaderboard(score, elapsedTime, errorsClassification.Length, vehicleType);
        }

        Stats.SetErrors(0);

        GameManager.Instance.setDashboardVisibility(false);
        showPopup(finishSet);
    }

    public void showPopup(Transform set, bool pauseGame = true)
    {
        hideAllPopups();

        set.gameObject.SetActive(true);
        lastActiveSet = set;
        popUpBox.gameObject.SetActive(true);

        if (pauseGame) {
            GameManager.Instance.pauseGame();
        }
        Debug.Log("show Popup");
    }
    public void closePopup()
    {
        Debug.Log("Close Popup");
        if (lastActiveSet == null) {
            return;
        }

        lastActiveSet.gameObject.SetActive(false);
        popUpBox.gameObject.SetActive(false);
        throttleUI.GetComponent<ThrottleUI>().ResetThrottle();
        headCheckUI.GetComponent<HeadCheck>().resetHeadCheck();

        GameManager.Instance.resumeGame();
    }

    public void setErrorBehavior(ErrorBehavior b)
    {
        Transform errorSet = popUpSystem.transform.Find("errorSet");
        if (errorSet == null) {
            Debug.Log("NO ERROR SET");
            return;
        }

        Debug.Log("Set ERROR BEHAVIOR");
        GameObject resetButton = errorSet.Find("resetButton").gameObject;
        GameObject loadSaveStateButton = errorSet.Find("loadSaveStateButton").gameObject;
        if (b == ErrorBehavior.LoadSave) {
            loadSaveStateButton?.SetActive(true);
            resetButton?.SetActive(false);
        } else {
            loadSaveStateButton?.SetActive(false);
            resetButton?.SetActive(true);
        }
    }

    void setHeaderText(Transform set, string headerMessage)
    {
        GameObject headerTextObject = set.Find("headerText").gameObject;
        TextMeshProUGUI headerText = headerTextObject.GetComponent<TextMeshProUGUI>();
        headerText.text = headerMessage;
    }
    void setBodyText(Transform set, string bodyMessage)
    {
        GameObject bodyTextObject = set.Find("bodyText").gameObject;
        TextMeshProUGUI bodyText = bodyTextObject.GetComponent<TextMeshProUGUI>();

        bodyText.text = bodyMessage;
    }
    void setFinishText(Transform set, float speed, float elapsedTime, int errors, string[] errorsClassification, int score = 0)
    {
        GameObject speedTextObject = set.Find("speed").gameObject;
        TextMeshProUGUI speedText = speedTextObject.GetComponent<TextMeshProUGUI>();
        GameObject timeTextObject = set.Find("time").gameObject;
        TextMeshProUGUI timeText = timeTextObject.GetComponent<TextMeshProUGUI>();
        GameObject scoresTextObject = set.Find("score").gameObject;
        TextMeshProUGUI scoresText = scoresTextObject.GetComponent<TextMeshProUGUI>();
        GameObject mistakesTextObjectParent = set.Find("Mistakes").gameObject;
        GameObject mistakesTextObject = mistakesTextObjectParent.transform.Find("mistakes").gameObject;
        TextMeshProUGUI mistakesText = mistakesTextObject.GetComponent<TextMeshProUGUI>();


        string[] statTexts = Stats.formatStats(speed, elapsedTime, errors, errorsClassification);
        timeText.text = statTexts[0];

        speedText.text = statTexts[1];
        scoresText.text = score.ToString();
        mistakesText.text = statTexts[3].Replace("\t", "");
    }

    void hideAllPopups()
    {
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void addUserError(string errorClassification)
    {
        Stats.AddUserError(errorClassification);
    }
}
