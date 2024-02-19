using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blinkers : MonoBehaviour
{
    public GameObject blinkerGroup;

    private CanvasGroup left;
    private CanvasGroup right;

    private int leftStatus;
    private int rightStatus;

    private float timerLeft;
    private float timerRight;

    void Start()
    {
        GameObject blinkerLeft = blinkerGroup.transform.GetChild(0).gameObject;
        GameObject blinkerRight = blinkerGroup.transform.GetChild(1).gameObject;

        left = blinkerLeft.GetComponent<CanvasGroup>();
        right = blinkerRight.GetComponent<CanvasGroup>();

        leftStatus = 0;
        rightStatus = 0;

        timerLeft = 0f;
        timerRight = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        left.alpha = 0.1f;
        right.alpha = 0.1f;
        if (Input.GetKeyDown("q"))
        {
            if (leftStatus == 1){
                leftStatus = 0;
            }
            else {
                leftStatus = 1;
                rightStatus = 0;
            }
        }
        if (Input.GetKeyDown("e"))
        {
            if (rightStatus == 1){
                rightStatus = 0;
            }
            else {
                rightStatus = 1;
                leftStatus = 0;
            }
        }

        if (Input.GetKey("a") != true && timerLeft >= 1){
            leftStatus = 0;
        }
        if (Input.GetKey("d") != true && timerRight >= 1){
            rightStatus = 0;
        }

        if (Input.GetKey("a")){
            timerLeft += Time.deltaTime;
        }
        else{
            timerLeft = 0f;
        }

        if (Input.GetKey("d")){
            timerRight += Time.deltaTime;
        }
        else{
            timerRight = 0f;
        }

        if (leftStatus == 1)
        {
            left.alpha = 1f;
        }
        if (rightStatus == 1)
        {
            right.alpha = 1f;
        }
    }
}
