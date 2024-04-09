using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class popUp : MonoBehaviour
{
    public GameObject popUpSystem;

    private Transform lastActiveSet;
    private Transform popUpBox;
    void Start()
    {
        lastActiveSet = null;
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

    public void popStart(string headerMessage, string bodyMessage)
    {
        Transform startSet = popUpSystem.transform.Find("startSet");

        GameObject headerTextObject = startSet.Find("headerText").gameObject;
        GameObject bodyTextObject = startSet.Find("bodyText").gameObject;
        TextMeshProUGUI headerText = headerTextObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bodyText = bodyTextObject.GetComponent<TextMeshProUGUI>();

        headerText.text = headerMessage;
        bodyText.text = bodyMessage;

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

        GameObject headerTextObject = promptSet.Find("headerText").gameObject;
        GameObject bodyTextObject = promptSet.Find("bodyText").gameObject;
        TextMeshProUGUI headerText = headerTextObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bodyText = bodyTextObject.GetComponent<TextMeshProUGUI>();

        headerText.text = headerMessage;
        bodyText.text = bodyMessage;

        showPopup(promptSet);
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
}
