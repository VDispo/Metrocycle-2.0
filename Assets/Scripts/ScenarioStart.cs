using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioStart : MonoBehaviour
{
    [TextArea(3, 10)] public string scenarioTitle;
    [TextArea(3, 10)] public string scenarioText;

    // Update is called once per frame
    void Start()
    {
        GameManager.Instance.PopupSystem.popStart(scenarioTitle, scenarioText);
    }
}
