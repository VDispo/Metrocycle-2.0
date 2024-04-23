using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    LEFT,
    RIGHT
};

public enum BlinkerStatus {
    OFF = 0,
    ON = 1
};

public class blinkers : MonoBehaviour
{
    public GameObject blinkerGroup;
    public GameObject bike;
    public int blinkerAutoOffAngle;
    public float blinkDuration = 0.5f;

    public int leftStatus;
    public int rightStatus;

    public float blinkerActivationTime;
    public float blinkerOffTime;
    public Direction lastActiveBlinker;

    public float maxUncancelledBlinkerTime = 5f;

    private CanvasGroup left;
    private CanvasGroup right;
    private float blinkTimer;
    private Vector3 prevRotation;
    private double turnAngle;
    private float shouldCancelAtTime;

    void Start()
    {
        GameObject blinkerLeft = blinkerGroup.transform.GetChild(0).gameObject;
        GameObject blinkerRight = blinkerGroup.transform.GetChild(1).gameObject;

        left = blinkerLeft.GetComponent<CanvasGroup>();
        right = blinkerRight.GetComponent<CanvasGroup>();

        leftStatus = 0;
        rightStatus = 0;

        blinkTimer = 0f;

        left.alpha = 0.1f;
        right.alpha = 0.1f;

        prevRotation = new Vector3(0, 0, 0);
        turnAngle = 0f;

        blinkerActivationTime = -1;
        blinkerOffTime = -1;

        shouldCancelAtTime = -1;
    }

    void setBlinker(Direction which, BlinkerStatus status) {
        int own_status, other_status;
        float own_alpha, other_alpha;
        lastActiveBlinker = which;

        if (status == BlinkerStatus.ON) {
            own_status = 1;
            own_alpha = 1f;
            other_status = 0;
            other_alpha = 0.1f;

            prevRotation = bike.transform.eulerAngles;
            turnAngle = 0f;

            blinkerActivationTime = Time.time;
            blinkerOffTime = -1;
        }
        else {
            own_status = 0;
            own_alpha = 0.1f;
            other_status = 1;
            other_alpha = 1f;
            blinkerOffTime = Time.time;
        }

        if (which == Direction.LEFT) {
            leftStatus = own_status;
            left.alpha = own_alpha;
            if (status == BlinkerStatus.ON) {
                rightStatus = other_status;
                right.alpha = other_alpha;
            }
        } else {
            rightStatus = own_status;
            right.alpha = own_alpha;
            if (status == BlinkerStatus.ON) {
                leftStatus = other_status;
                left.alpha = other_alpha;
            }
        }
    }

    void animateBlinker() {
        // BLINKER LOGIC
        blinkTimer += Time.deltaTime;
        if (leftStatus == 1)
        {
            if (blinkTimer >= blinkDuration){
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
            if (blinkTimer >= blinkDuration){
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

    bool checkAutoBlinkerOff(Direction which) {
        if ( (which == Direction.RIGHT && turnAngle > blinkerAutoOffAngle)
            || (which == Direction.LEFT && -turnAngle > blinkerAutoOffAngle)
        ) {
            // Debug.Log("Direction OFF.");
            // if (leftStatus == 1)
            //     setBlinker(Direction.LEFT, BlinkerStatus.OFF);
            // if (rightStatus == 1)
            //     setBlinker(Direction.RIGHT, BlinkerStatus.OFF);
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {   
        // TURNING ON BLINKER
        if (Input.GetKeyDown("q"))
        {
            if (leftStatus == 1){
                setBlinker(Direction.LEFT, BlinkerStatus.OFF);
            }
            else {
                setBlinker(Direction.LEFT, BlinkerStatus.ON);
                blinkTimer = 0;
            }
        }
        if (Input.GetKeyDown("e"))
        {
            if (rightStatus == 1){
                setBlinker(Direction.RIGHT, BlinkerStatus.OFF);
            }
            else {
                setBlinker(Direction.RIGHT, BlinkerStatus.ON);
                blinkTimer = 0;
            }
        }


        bool hasHorizontalInput = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;

        // Update turnAngle
        if (leftStatus == 1 || rightStatus == 1) {
            Vector3 curRotation = bike.transform.eulerAngles;
            turnAngle += Mathf.DeltaAngle(prevRotation.y, curRotation.y);
            prevRotation = curRotation;

            // Detect when blinker should be off only when turn buttons are not pressed
            if (hasHorizontalInput) {
                Direction which = leftStatus == 1 ? Direction.LEFT : Direction.RIGHT;
                if (shouldCancelAtTime == -1 && checkAutoBlinkerOff(which)) {
                    shouldCancelAtTime = Time.time;
                    Debug.Log("Should autooff now " + shouldCancelAtTime);
                }

            }

            if (shouldCancelAtTime != -1) {
                if (hasHorizontalInput) {
                    // reset timer when turn button is pressed
                    shouldCancelAtTime = Time.time;
                } else {
                    if (Time.time - shouldCancelAtTime > maxUncancelledBlinkerTime) {
                        if (GameManager.Instance.PopupSystem != null) {
                            GameManager.Instance.PopupSystem.popPrompt("You forgot to cancel your blinker",
                                "Make sure to cancel your " + GameManager.Instance.blinkerName() + " after performing a turn.");
                        }

                        shouldCancelAtTime = -1;
                    }
                }
            }

        }

        animateBlinker();
    }
}
