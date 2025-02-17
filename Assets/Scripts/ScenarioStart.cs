using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metrocycle;

public class ScenarioStart : MonoBehaviour
{
    public BikeType bikeType = BikeType.Motorcycle;
    public bool isTestScene = false;
    public string ScenarioTitle { get; set; }
    public string ScenarioText { get; set; }

    // Update is called once per frame
    void Start()
    {
        GameManager.Instance.PopupSystem.popStart(ScenarioTitle, ScenarioText);
        GameManager.Instance.setBikeType(bikeType);
        GameManager.Instance.isTestMode = isTestScene;
    }

    public void pauseGame()
    {
        GameManager.Instance.PopupSystem.popPause();
    }
}
