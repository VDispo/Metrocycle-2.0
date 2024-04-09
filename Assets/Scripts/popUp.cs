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
    public GameObject popUpSystem;

    private Transform lastActiveSet = null;
    private Transform popUpBox;

    delegate void PopDelegate(string header, string body);
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
                    });
                } else if (button.name.StartsWith("reset")) {
                    buttonScript.onClick.AddListener(() => {
                        closePopup(); GameManager.Instance.restartGame();
                    });
                } else if (button.name.StartsWith("menu")) {
                    buttonScript.onClick.AddListener(() => {
                        closePopup(); SceneManager.LoadScene("Main Menu");
                    });
                }
            }
        }
    }

    public void popWithType(PopupType type, string headerMessage, string bodyMessage)
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
                break;
            case PopupType.PROMPT:
                popFunc = popPrompt;
                break;
            default:
                Debug.LogError("popUp: Only START, ERROR, PROMPT supported for now");
                return;
        }

        if (popFunc != null) {
            popFunc(headerMessage, bodyMessage);
        } else {

        }
    }

    public void popStart(string headerMessage, string bodyMessage)
    {
        Transform startSet = popUpSystem.transform.Find("startSet");

        setHeaderText(startSet, headerMessage);
        setBodyText(startSet, bodyMessage);

        showPopup(startSet);
    }

    public void popPause()
    {
        Transform pauseSet = popUpSystem.transform.Find("pauseSet");
        showPopup(pauseSet);
    }

    public void popPrompt(string headerMessage, string bodyMessage)
    {
        Transform promptSet = popUpSystem.transform.Find("promptSet");

        setHeaderText(promptSet, headerMessage);
        setBodyText(promptSet, bodyMessage);

        showPopup(promptSet);
    }

    public void popError(string headerMessage, string bodyMessage)
    {
        Transform errorSet = popUpSystem.transform.Find("errorSet");

        setHeaderText(errorSet, headerMessage);
        setBodyText(errorSet, bodyMessage);

        showPopup(errorSet);
    }

    public void popFinish()
    {
        Transform finishSet = popUpSystem.transform.Find("finishSet");
        showPopup(finishSet);
    }

    public void showPopup(Transform set, bool pauseGame = true)
    {
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

        GameManager.Instance.resumeGame();
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
}
