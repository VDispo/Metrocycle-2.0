using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProgression : MonoBehaviour
{
    public GameObject blinkerUI;
    public GameObject headCheckUI;
    public GameObject leftBrakeUI;
    public GameObject rightBrakeUI;

    public GameObject leftHeadCheckUI;
    public GameObject rightHeadCheckUI;
    public GameObject upHeadCheckUI;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tutorialProgress(int progress)
    {
        Debug.Log("Progress: " + progress);
        switch (progress)
        {
            case 1:
                BlinkerUIOn();
                break;
            case 5:
                HeadCheckUIOn();
                break;
            case 8:
                RightHeadCheckUIOn();
                break;
            case 10:
                BrakeUIOn();
                break;
            default:
                Debug.Log("No UI to show for this progress level.");
                break;
        }
    }

    void BlinkerUIOn()
    {
        blinkerUI.SetActive(true);
    }

    void HeadCheckUIOn()
    {
        headCheckUI.SetActive(true);
        leftHeadCheckUI.SetActive(true);
        rightHeadCheckUI.SetActive(false);
        upHeadCheckUI.SetActive(false);
    }
    void RightHeadCheckUIOn()
    {
        rightHeadCheckUI.SetActive(true);
    }

    void BrakeUIOn()
    {
        leftBrakeUI.SetActive(true);
        rightBrakeUI.SetActive(true);
        upHeadCheckUI.SetActive(true);
    }
}
