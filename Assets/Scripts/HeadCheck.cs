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
    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        if (!brain.IsBlending) {
            if (Input.GetKeyDown("1") && normal.Priority == 20)
            {
                resetPriorities();
                left.Priority = 20;
                leftCheckTime = Time.time;
            } else if (Input.GetKeyDown("3") && normal.Priority == 20)
            {
                resetPriorities();
                right.Priority = 20;
                rightCheckTime = Time.time;
            } else if (Input.GetKeyDown("2")) {
                resetPriorities();
                normal.Priority = 20;
            }
        }
    }
}
