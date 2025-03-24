using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialConditionsSelector : MonoBehaviour
{
    public SpecialConditionsInitializer SpecialConditionsInitializer;

    public void ActivateRainCondition(TextMeshProUGUI text)
    {
        SpecialConditionsInitializer.specialConditionsInvolved["Rain"] = IsActivatedAndToggleButton(text);
    }

    // rough coding for now
    public bool IsActivatedAndToggleButton(TextMeshProUGUI text)
    {
        string[] buttonTextParts = text.text.Split(" ");
        bool isActivated = !(buttonTextParts[1] == "Enabled"); // toggle
        text.text = buttonTextParts[0] + (isActivated ? " Enabled " : " Disabled ") + buttonTextParts[2];
        return isActivated;
    }
}
