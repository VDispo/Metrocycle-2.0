using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarHandler : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject checkpoints;
    private int numCheckpoints;
    private int currCheckpointIdx;
    
    
    // Start is called before the first frame update
    void Start()
    {
        progressBar = GetComponent<Slider>();
        if (checkpoints) {
            numCheckpoints = checkpoints.transform.childCount;
        }
        Debug.Log($"Number of checkpoints: {numCheckpoints}");
    }

    // Update is called once per frame
    void Update()
    {
        if (checkpoints && currCheckpointIdx != GetDisabledChildCount()) {
            currCheckpointIdx = GetDisabledChildCount();
            numCheckpoints = GetCheckpointCount();
            Debug.Log($"Current checkpoint index: {currCheckpointIdx} / {numCheckpoints}");
            progressBar.value = (float)currCheckpointIdx / numCheckpoints;
        }
    }
    int GetCheckpointCount() 
    {
        return checkpoints.transform.childCount;
    }

    int GetDisabledChildCount() 
    {
        int disabledCount = 0;
        for (int i = 0; i < checkpoints.transform.childCount; ++i) {
            if (!checkpoints.transform.GetChild(i).gameObject.activeSelf) {
                disabledCount++;
            }
        }
        return disabledCount;
    }
}
