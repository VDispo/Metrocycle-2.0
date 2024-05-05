using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameoverPopup : MonoBehaviour
{
    public GameObject speedometer;
    public GameObject timer;
    public GameObject textElement;

    public void popupShown()
    {
        // double avgSpeed = System.Math.Round(GetAvgSpeed(), 2);
        double avgSpeed = Speedometer.GetAvgSpeed();
        avgSpeed = System.Math.Round(avgSpeed, 2);
        Text  timerText = timer.GetComponent<Text>();
        string[] time_comps = timerText.text.Split(":");

        TextMeshProUGUI text = textElement.GetComponent<TextMeshProUGUI>();
        text.text = string.Format("Time finished: {0:00}m{1:00}s\nAverage Speed: {2} km/s", time_comps[0], time_comps[1], avgSpeed);
    }
}
