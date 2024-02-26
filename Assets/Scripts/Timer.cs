using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    float currentTime;
    public Text currentTimeText;

    // Start is called before the first frame update
    void Start()
    {
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
