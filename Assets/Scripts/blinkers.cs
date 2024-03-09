using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Blinker {
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
    public GameObject motorbike;
    public int blinkerAutoOffAngle;
    // NOTE: made public for easier debugging
    // TODO: change to private
    public Vector3 prevRotation;
    public double turnAngle;
    public int leftStatus;
    public int rightStatus;

    private CanvasGroup left;
    private CanvasGroup right;
    private float blinkTimer;

    public float blinkerActivationTime;
    public float blinkerOffTime;
    public Blinker lastActiveBlinker;

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
    }

    void setBlinker(Blinker which, BlinkerStatus status) {
        int own_status, other_status;
        float own_alpha, other_alpha;
        lastActiveBlinker = which;

        if (status == BlinkerStatus.ON) {
            own_status = 1;
            own_alpha = 1f;
            other_status = 0;
            other_alpha = 0.1f;

            prevRotation = motorbike.transform.eulerAngles;
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

        if (which == Blinker.LEFT) {
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

    void checkAutoBlinkerOff(Blinker which) {
        if ( (which == Blinker.RIGHT && turnAngle > blinkerAutoOffAngle)
            || (which == Blinker.LEFT && -turnAngle > blinkerAutoOffAngle)
        ) {
            Debug.Log("Blinker OFF.");
            if (leftStatus == 1)
                setBlinker(Blinker.LEFT, BlinkerStatus.OFF);
            if (rightStatus == 1)
                setBlinker(Blinker.RIGHT, BlinkerStatus.OFF);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // TURNING ON BLINKER
        if (Input.GetKeyDown("q"))
        {
            if (leftStatus == 1){
                setBlinker(Blinker.LEFT, BlinkerStatus.OFF);
            }
            else {
                setBlinker(Blinker.LEFT, BlinkerStatus.ON);
                blinkTimer = 0;
            }
        }
        if (Input.GetKeyDown("e"))
        {
            if (rightStatus == 1){
                setBlinker(Blinker.RIGHT, BlinkerStatus.OFF);
            }
            else {
                setBlinker(Blinker.RIGHT, BlinkerStatus.ON);
                blinkTimer = 0;
            }
        }

        // Update turnAngle
        if (leftStatus == 1 || rightStatus == 1) {
            Vector3 curRotation = motorbike.transform.eulerAngles;
            turnAngle += Mathf.DeltaAngle(prevRotation.y, curRotation.y);
            prevRotation = curRotation;

            // only turn blinker off automatically when
            // turn buttons are not pressed
            if (Input.GetAxis("Horizontal") != 1) {
                Blinker which = leftStatus == 1 ? Blinker.LEFT : Blinker.RIGHT;
                checkAutoBlinkerOff(which);
            }
        }

        animateBlinker();
    }
}
