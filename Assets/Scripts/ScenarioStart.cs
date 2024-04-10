using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metrocycle;

public class ScenarioStart : MonoBehaviour
{
    public BikeType bikeType = BikeType.Motorcycle;
    [TextArea(3, 10)] public string scenarioTitle;
    [TextArea(3, 10)] public string scenarioText;

    // Update is called once per frame
    void Start()
    {
        GameManager.Instance.PopupSystem.popStart(scenarioTitle, scenarioText);
        GameManager.Instance.setBikeType(bikeType);
    }
}
