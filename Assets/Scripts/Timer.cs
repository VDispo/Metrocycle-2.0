using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    float currentTime;
    public Text currentTimeText;

    // TRYING OUT STATS //
    private static string filePath;
    //////////////////////

    // Start is called before the first frame update
    void Start()
    {
        // TRYING OUT STATS //
        string sceneId = "Scenario1";
        filePath = "Assets/PlayerStats_" + sceneId + ".json";
        Stats.SaveSpeed(100.0f, sceneId);
        //////////////////////

        currentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = currentTime + Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        if (time.Seconds.ToString().Length == 1){
            currentTimeText.text = time.Minutes.ToString() + ":0" + time.Seconds.ToString();
        }
        else{
            currentTimeText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString();
        }
    }
}
