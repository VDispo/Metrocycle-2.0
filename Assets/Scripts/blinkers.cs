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

    private float blinkTimer;

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

        blinkTimer = 0f;

        left.alpha = 0.1f;
        right.alpha = 0.1f;
    }


    // Update is called once per frame
    void Update()
    {   
        // TURNING ON BLINKER
        if (Input.GetKeyDown("q"))
        {
            if (leftStatus == 1){
                leftStatus = 0;
                left.alpha = 0.1f;
            }
            else {
                leftStatus = 1;
                left.alpha = 1f;
                blinkTimer = 0;
                rightStatus = 0;
                right.alpha = 0.1f;
            }
        }
        if (Input.GetKeyDown("e"))
        {
            if (rightStatus == 1){
                rightStatus = 0;
                right.alpha = 0.1f;
            }
            else {
                rightStatus = 1;
                right.alpha = 1f;
                blinkTimer = 0;
                leftStatus = 0;
                left.alpha = 0.1f;
            }
        }

        // TURNING INTO BLINKER OFF
        if (Input.GetKey("a") != true && timerLeft >= 1){
            leftStatus = 0;
            left.alpha = 0.1f;
        }
        if (Input.GetKey("d") != true && timerRight >= 1){
            rightStatus = 0;
            right.alpha = 0.1f;
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
        
        // BLINKER LOGIC
        blinkTimer += Time.deltaTime;
        if (leftStatus == 1)
        {
            if (blinkTimer >=1){
                if (left.alpha == 1f){
                    left.alpha = 0.1f;
                }
                else{
                    left.alpha = 1f;
                }
                blinkTimer = 0f;
            }
        }
        if (rightStatus == 1)
        {
            if (blinkTimer >=1){
                if (right.alpha == 1f){
                    right.alpha = 0.1f;
                }
                else{
                    right.alpha = 1f;
                }
                blinkTimer = 0f;
            }
        }
    }
}
