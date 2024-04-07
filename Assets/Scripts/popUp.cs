using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class popUp : MonoBehaviour
{
    public GameObject popUpSystem;

    public void popStart(string headerMessage, string bodyMessage)
    {
        Transform startSet = popUpSystem.transform.Find("startSet");

        GameObject headerTextObject = startSet.Find("headerText").gameObject;
        GameObject bodyTextObject = startSet.Find("bodyText").gameObject;
        TextMeshProUGUI headerText = headerTextObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bodyText = bodyTextObject.GetComponent<TextMeshProUGUI>();

        headerText.text = headerMessage;
        bodyText.text = bodyMessage;

        startSet.gameObject.SetActive(true);
    }

    public void popPause()
    {
        Transform pauseSet = popUpSystem.transform.Find("pauseSet");
        pauseSet.gameObject.SetActive(true);
    }

    public void popPrompt(string message)
    {
        Transform promptSet = popUpSystem.transform.Find("promptSet");
        promptSet.gameObject.SetActive(true);
    }

    public void popFinish()
    {
        Transform finishSet = popUpSystem.transform.Find("finishSet");
        finishSet.gameObject.SetActive(true);
    }


    void Start()
    {
        popFinish();
    }
}
