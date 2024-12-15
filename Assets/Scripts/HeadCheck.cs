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
        // Headturning controls
        bool unpressingButton, pressingLeftHeadCheck, pressingRightHeadCheck;
        if (IsAndroid)
        {
            unpressingButton = leftHeadCheckButton.GetComponent<HeadCheckUI>().simulateKeyUp || rightHeadCheckButton.GetComponent<HeadCheckUI>().simulateKeyUp;
            if (unpressingButton)
            {
                leftHeadCheckButton.GetComponent<HeadCheckUI>().simulateKeyUp = false;
                rightHeadCheckButton.GetComponent<HeadCheckUI>().simulateKeyUp = false;
            }
            pressingLeftHeadCheck = leftHeadCheckButton.GetComponent<HeadCheckUI>().isButtonPressed;
            pressingRightHeadCheck = rightHeadCheckButton.GetComponent<HeadCheckUI>().isButtonPressed && lastView != left;
        } 
        else
        {
            unpressingButton = Input.GetKeyUp(KeyCode.J) || Input.GetKeyUp(KeyCode.K) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1);
            pressingLeftHeadCheck = Input.GetKey(KeyCode.J) || Input.GetMouseButton(0);
            pressingRightHeadCheck = Input.GetKey(KeyCode.K) || Input.GetMouseButton(1);
        }

        // Headturning variables
        if (unpressingButton)
        {
            resetPriorities();
            normal.Priority = 20;
        }
        else if (isLookingForward() && !brain.IsBlending
                    && ((lastView == left && Time.time - leftCheckTime > headCheckSpeed)
                        || (lastView == right && Time.time - rightCheckTime > headCheckSpeed)))
        {
            lastView = normal;
        }

        if (pressingLeftHeadCheck && lastView != right)
        {
            normal.Priority = 10;
            right.Priority = 10;
            left.Priority = 20;
            leftCheckTime = Time.time;
            lastView = left;

        }
        else if (pressingRightHeadCheck && lastView != left)
        {
            left.Priority = 10;
            normal.Priority = 10;
            right.Priority = 20;
            rightCheckTime = Time.time;
            lastView = right;
        }
    }
}
