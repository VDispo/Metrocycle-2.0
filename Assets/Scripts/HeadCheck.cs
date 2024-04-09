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


    [HideInInspector] public float leftCheckTime;
    [HideInInspector] public float rightCheckTime;

    private CinemachineBrain brain;

    void Start()
    {
        brain = gameObject.GetComponent<CinemachineBrain>();

        resetPriorities();
        normal.Priority = 20;

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
        return left.Priority == 20;
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
        if (Input.GetKeyUp("1") || Input.GetKeyUp("3")) {
            resetPriorities();
            normal.Priority = 20;
        }

        if (Input.GetKey("1") && !isLookingRight())
        {
            left.Priority = 20;
            leftCheckTime = Time.time;

        } else if (Input.GetKey("3") && !isLookingLeft())
        {
            right.Priority = 20;
            rightCheckTime = Time.time;
        }
    }
}
