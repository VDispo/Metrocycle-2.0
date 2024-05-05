using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    public static float CurrentTime;
    public Text currentTimeText;

    // Start is called before the first frame update
    void Start()
    {
        CurrentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentTime = CurrentTime + Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(CurrentTime);
        if (time.Seconds.ToString().Length == 1){
            currentTimeText.text = time.Minutes.ToString() + ":0" + time.Seconds.ToString();
        }
        else{
            currentTimeText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString();
        }
    }
}
