using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChangeLaneDetect : MonoBehaviour
{
    [SerializeField]
    public GameObject[] allLaneHolders;
    [SerializeField]
    public GameObject[] adjacentLaneHolders;

    void Start() {
        allLaneHolders = (allLaneHolders.ToList()
                          .Except(adjacentLaneHolders.ToList())
                          .ToArray());
    }

    void OnTriggerEnter (Collider other) {
        GameObject currentLaneHolder = gameObject;
        Debug.Log("Entered Lane", currentLaneHolder);
        // Disable all colliders in this lane since we don't
        //   need to check that we are in the same lane
        currentLaneHolder.SetActive(false);

        // Disable all lane colliders
        foreach (GameObject laneHolder in allLaneHolders) {
            laneHolder.SetActive(false);
        }

        // only enable adjacent lane colliders
        foreach (GameObject laneHolder in adjacentLaneHolders) {
            laneHolder.SetActive(true);
        }
    }
}
