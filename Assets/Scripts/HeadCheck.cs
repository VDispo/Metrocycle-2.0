using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HeadCheck : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera normal;
    [SerializeField] public CinemachineVirtualCamera right;
    [SerializeField] public CinemachineVirtualCamera left;
    [SerializeField] public float headCheckSpeed;
    // Turn must be made within reasonable time after head check
    [SerializeField] public float maxHeadCheckDelay = 5f;

    [HideInInspector] public float leftCheckTime;
    [HideInInspector] public float rightCheckTime;

    public Button leftHeadCheckButton;
    public Button rightHeadCheckButton;

    private CinemachineBrain brain;
    private CinemachineVirtualCamera lastView;

    private bool IsAndroid = true;

    void Start()
    {  
        IsAndroid = Application.platform == RuntimePlatform.Android;
        IsAndroid = true; // For Android Build
        
        brain = gameObject.GetComponent<CinemachineBrain>();

        resetPriorities();
        normal.Priority = 20;
        lastView = normal;

        brain.m_DefaultBlend.m_Time = headCheckSpeed;

        leftCheckTime = -1;
        rightCheckTime = -1;

        GameManager.Instance.resetSignal.AddListener(() => {
            resetPriorities();

            leftCheckTime = -1;
            rightCheckTime = -1;
            lastView = normal;
       });
    }

    void resetPriorities()
    {
        normal.Priority = 10;
        right.Priority = 10;
        left.Priority = 10;
    }

    public bool isLookingRight() {
        return right.Priority == 20;
    }
    public bool isLookingLeft() {
        return left.Priority == 20;
    }
    public bool isLookingForward() {
        return normal.Priority == 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAndroid) {
            if (leftHeadCheckButton.GetComponent<HeadCheckUI>().simulateKeyUp || rightHeadCheckButton.GetComponent<HeadCheckUI>().simulateKeyUp) {
                leftHeadCheckButton.GetComponent<HeadCheckUI>().simulateKeyUp = false;
                rightHeadCheckButton.GetComponent<HeadCheckUI>().simulateKeyUp = false;
                resetPriorities();
                normal.Priority = 20;
            } else if (isLookingForward() && !brain.IsBlending
                        && ((lastView == left && Time.time - leftCheckTime > headCheckSpeed)
                            ||(lastView == right && Time.time - rightCheckTime > headCheckSpeed)
                        )
            ) {
                lastView = normal;
            }

            if (leftHeadCheckButton.GetComponent<HeadCheckUI>().isButtonPressed && !isLookingLeft()) {
                normal.Priority = 10;
                right.Priority = 10;
                left.Priority = 20;
                leftCheckTime = Time.time;
            } else if (rightHeadCheckButton.GetComponent<HeadCheckUI>().isButtonPressed && !isLookingRight()) {
                left.Priority = 10;
                normal.Priority = 10;
                right.Priority = 20;
                rightCheckTime = Time.time;
            }
        } else {
            if (Input.GetKeyUp("j") || Input.GetKeyUp("k") || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                resetPriorities();
                normal.Priority = 20;
            } else if (isLookingForward() && !brain.IsBlending
                        && ((lastView == left && Time.time - leftCheckTime > headCheckSpeed)
                            ||(lastView == right && Time.time - rightCheckTime > headCheckSpeed)
                        )
            ) {
                lastView = normal;
            }

            if ((Input.GetKey("j") || Input.GetMouseButton(0))&& lastView != right)
            {
                normal.Priority = 10;
                right.Priority = 10;
                left.Priority = 20;
                leftCheckTime = Time.time;

                lastView = left;

            } else if ((Input.GetKey("k") || Input.GetMouseButton(1)) && lastView != left)
            {
                left.Priority = 10;
                normal.Priority = 10;
                right.Priority = 20;
                rightCheckTime = Time.time;

                lastView = right;
            }
        }
    }
}
