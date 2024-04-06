using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class HeadCheck : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera normal;
    [SerializeField] public CinemachineVirtualCamera right;
    [SerializeField] public CinemachineVirtualCamera left;

    private CinemachineBrain brain;
    // Start is called before the first frame update
    void Start()
    {
        brain = gameObject.GetComponent<CinemachineBrain>();

        resetPriorities();
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
            } else if (Input.GetKeyDown("3") && normal.Priority == 20)
            {
                resetPriorities();
                right.Priority = 20;
            } else if (Input.GetKeyDown("2")) {
                resetPriorities();
                normal.Priority = 20;
            }
        }
    }
}
