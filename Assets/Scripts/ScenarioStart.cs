using UnityEngine;
using Metrocycle;
using TMPro;

public class ScenarioStart : MonoBehaviour
{
    public BikeType bikeType = BikeType.Motorcycle;
    public bool isTestScene = false;

    public TMP_Text objectiveTextUI;
    public string ScenarioTitle { get; set; }
    public string ScenarioText { get; set; }
    public string ObjectiveText { get; set; }

    void Start()
    {
        objectiveTextUI.text = ObjectiveText;
        GameManager.Instance.PopupSystem.popStart(ScenarioTitle, ScenarioText);
        GameManager.Instance.setBikeType(bikeType);
        GameManager.Instance.isTestMode = isTestScene;
    }

    public void pauseGame()
    {
        GameManager.Instance.PopupSystem.popPause();
    }
}
