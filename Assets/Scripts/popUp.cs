using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class popUp : MonoBehaviour
{
    public GameObject popUpSystem;

    public void popStart(string headerMessage, string bodyMessage)
    {
        Transform popUpBox = popUpSystem.transform.Find("popUpBox");
        Transform startSet = popUpSystem.transform.Find("startSet");

        GameObject headerTextObject = startSet.Find("headerText").gameObject;
        GameObject bodyTextObject = startSet.Find("bodyText").gameObject;
        TextMeshProUGUI headerText = headerTextObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bodyText = bodyTextObject.GetComponent<TextMeshProUGUI>();

        headerText.text = headerMessage;
        bodyText.text = bodyMessage;

        startSet.gameObject.SetActive(true);
        popUpBox.gameObject.SetActive(true);
    }

    public void popPause()
    {
        Transform popUpBox = popUpSystem.transform.Find("popUpBox");
        Transform pauseSet = popUpSystem.transform.Find("pauseSet");
        pauseSet.gameObject.SetActive(true);
        popUpBox.gameObject.SetActive(true);
    }

    public void popPrompt(string headerMessage, string bodyMessage)
    {
        Transform popUpBox = popUpSystem.transform.Find("popUpBox");
        Transform promptSet = popUpSystem.transform.Find("promptSet");

        GameObject headerTextObject = promptSet.Find("headerText").gameObject;
        GameObject bodyTextObject = promptSet.Find("bodyText").gameObject;
        TextMeshProUGUI headerText = headerTextObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bodyText = bodyTextObject.GetComponent<TextMeshProUGUI>();

        headerText.text = headerMessage;
        bodyText.text = bodyMessage;

        promptSet.gameObject.SetActive(true);
        popUpBox.gameObject.SetActive(true);
    }

    public void popFinish()
    {
        Transform popUpBox = popUpSystem.transform.Find("popUpBox");
        Transform finishSet = popUpSystem.transform.Find("finishSet");
        finishSet.gameObject.SetActive(true);
        popUpBox.gameObject.SetActive(true);
    }
}
