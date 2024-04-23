using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

    private CinemachineBrain brain;
    private CinemachineVirtualCamera lastView;

    void Start()
    {
        brain = gameObject.GetComponent<CinemachineBrain>();

        resetPriorities();
        normal.Priority = 20;
        lastView = normal;

        brain.m_DefaultBlend.m_Time = headCheckSpeed;

        leftCheckTime = -1;
        rightCheckTime = -1;
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
        if (Input.GetKeyUp("j") || Input.GetKeyUp("k")) {
            resetPriorities();
            normal.Priority = 20;
        } else if (isLookingForward() && !brain.IsBlending
                    && ((lastView == left && Time.time - leftCheckTime > headCheckSpeed)
                        ||(lastView == right && Time.time - rightCheckTime > headCheckSpeed)
                    )
        ) {
            lastView = normal;
        }

        if (Input.GetKey("j") && lastView != right)
        {
            normal.Priority = 10;
            right.Priority = 10;
            left.Priority = 20;
            leftCheckTime = Time.time;

            lastView = left;

        } else if (Input.GetKey("k") && lastView != left)
        {
            left.Priority = 10;
            normal.Priority = 10;
            right.Priority = 20;
            rightCheckTime = Time.time;

            lastView = right;
        }
    }
}
