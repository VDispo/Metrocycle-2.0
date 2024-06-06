using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    LEFT,
    RIGHT,
    FORWARD
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
    public int alphaOff;

    public float blinkerActivationTime;
    public float blinkerOffTime;
    public Direction lastActiveBlinker;

    public float maxUncancelledBlinkerTime = 5f;
    public float minBlinkerTime = 3f;
    // blinker can turn off early, within reasonable max
    public float maxBlinkerOffTime = 3f;

    private CanvasGroup left;
    private CanvasGroup right;
    private float blinkTimer;
    private Vector3 prevRotation;
    private double turnAngle;
    private float shouldCancelAtTime;

    private Metrocycle.BikeType bikeType;
    bool isBikeTypeSet = false;

    void Awake()
    {
        if (!isBikeTypeSet) {
            setBikeType(Metrocycle.BikeType.Motorcycle);
        }

        leftStatus = 0;
        rightStatus = 0;

        blinkTimer = 0f;

        left.alpha = 0.1f;
        right.alpha = 0.1f;
        alphaOff = 0;

        prevRotation = new Vector3(0, 0, 0);
        turnAngle = 0f;

        blinkerActivationTime = -1;
        blinkerOffTime = -1;

        shouldCancelAtTime = -1;

        GameManager.Instance.resetSignal.AddListener(() => {
            setBlinker(Direction.LEFT, BlinkerStatus.OFF);
            setBlinker(Direction.RIGHT, BlinkerStatus.OFF);
        });
    }

    public void setBikeType(Metrocycle.BikeType newBikeType) {
        int leftIdx = 0;
        if (newBikeType == Metrocycle.BikeType.Bicycle) {
            // NOTE: in blinker prefab, motor left and right icons are first 2 children,
            //       bike left and right icons are third and 4th children
            leftIdx += 2;
            alphaOff = 1;
        }

        for (int i = 0; i < blinkerGroup.transform.childCount; ++i) {
            blinkerGroup.transform.GetChild(i).gameObject.SetActive(false);
        }

        GameObject blinkerLeft = blinkerGroup.transform.GetChild(leftIdx).gameObject;
        GameObject blinkerRight = blinkerGroup.transform.GetChild(leftIdx+1).gameObject;

        blinkerLeft.SetActive(true);
        blinkerRight.SetActive(true);

        left = blinkerLeft.GetComponent<CanvasGroup>();
        right = blinkerRight.GetComponent<CanvasGroup>();
        left.alpha = (alphaOff == 0) ? 0.1f : 0f;
        right.alpha = (alphaOff == 0) ? 0.1f : 0f;

        bikeType = newBikeType;
        isBikeTypeSet = true;
    }

    void setBlinker(Direction which, BlinkerStatus status) {
        Debug.Assert(which != Direction.FORWARD);

        int own_status, other_status;
        float own_alpha, other_alpha;
        lastActiveBlinker = which;

        if (status == BlinkerStatus.ON) {
            own_status = 1;
            own_alpha = 1f;
            other_status = 0;
            other_alpha = (alphaOff == 0) ? 0.1f : 0f;

            prevRotation = bike.transform.eulerAngles;
            turnAngle = 0f;

            blinkerActivationTime = Time.time;
            blinkerOffTime = -1;
        }
        else {
            own_status = 0;
            own_alpha = (alphaOff == 0) ? 0.1f : 0f;
            other_status = 1;
            other_alpha = 1f;
            blinkerOffTime = Time.time;
        }

        shouldCancelAtTime = -1;

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
        Debug.Assert(which != Direction.FORWARD);

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

    public void startBlinkerCancelTimer()
    {
        if (shouldCancelAtTime == -1
            && (leftStatus == 1 || rightStatus == 1)
        )
        {
            shouldCancelAtTime = Time.time;
        }
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
                    // shouldCancelAtTime = Time.time;
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

                        GameManager.Instance.setErrorReason(Metrocycle.ErrorReason.UNCANCELLED_BLINKER);

                        shouldCancelAtTime = -1;
                    }
                }
            }

        }

        if (bikeType == Metrocycle.BikeType.Motorcycle) {
            animateBlinker();
        }
    }
}
