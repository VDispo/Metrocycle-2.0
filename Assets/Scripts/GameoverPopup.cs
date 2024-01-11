using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameoverPopup : MonoBehaviour
{
    public GameObject speedometer;
    public GameObject textElement;

    public void popupShown()
    {
        // double avgSpeed = System.Math.Round(GetAvgSpeed(), 2);
        double avgSpeed = speedometer.GetComponent<Speedometer>().GetAvgSpeed();
        avgSpeed = System.Math.Round(avgSpeed, 2);
        Debug.Log("Popup shown " + avgSpeed );
        TextMeshProUGUI text = textElement.GetComponent<TextMeshProUGUI>();
        text.text = "Time finished:\nAverage Speed: " + avgSpeed + "km/s";
    }
}
