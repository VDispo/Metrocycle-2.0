using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script sits in the select mode menu screen, particularly when selecting the specific modes (Intersection, EDSA, etc.).<br/>
/// It saves the player's choices into a <see cref="SpecialConditionsSelected"/> instance, which is instantiated here at runtime and persists through the next scene change.
/// </summary>
public class SpecialConditionsSelector : MonoBehaviour
{
    private SpecialConditionsSelected specialConditionsSelected;

    private void Start()
    {
        specialConditionsSelected = Instantiate(new GameObject()).AddComponent<SpecialConditionsSelected>();
        if (!specialConditionsSelected) specialConditionsSelected = SpecialConditionsSelected.Instance;
    }

    public void ActivateNightCondition(Button button)
    {
        specialConditionsSelected.conditions["Night"] = !specialConditionsSelected.conditions["Night"];
        ActivateButton(button);
        Debug.Log("Night = " + specialConditionsSelected.conditions["Night"]);
    }

    public void ActivateRainCondition(Button button)
    {
        specialConditionsSelected.conditions["Rain"] = !specialConditionsSelected.conditions["Rain"];
        ActivateButton(button);
        Debug.Log("Rain = " + specialConditionsSelected.conditions["Rain"]);
    }

    public void ActivateFogCondition(Button button)
    {
        specialConditionsSelected.conditions["Fog"] = !specialConditionsSelected.conditions["Fog"];
        ActivateButton(button);
        Debug.Log("Fog = " + specialConditionsSelected.conditions["Fog"]);
    }

    public void ActivateButton(Button button)
    {
        Image img = button.GetComponent<Image>();
        img.color = img.color == Color.white ? Color.gray : Color.white; // darken (white is false, gray is true)
    }
}
