using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSequence : MonoBehaviour
{
    public GameObject finishLine = null;
    [HideInInspector]
    public GameObject[] events;
    public Vector3 offsetFromEventDetect;

    private int curEventIdx;

    // Start is called before the first frame update
    void Start()
    {
        events = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i) {
            Transform eventTransform = transform.GetChild(i);
            events[i] = eventTransform.gameObject;
            events[i].SetActive(false);

            // Install moveToNextEvent as callback when event is reached
            // HACK: Assume first child of event is the SUCCESS detect
            if (eventTransform.childCount < 1) {
                // TODO: Event without SUCCESS detect? Probably unused. Skip for now
                continue;
            }
            CheckpointDetection detectScript = eventTransform.GetChild(0).GetComponent<CheckpointDetection>();
            detectScript.triggerSignal.AddListener(moveToNextEvent);
        }

        curEventIdx = 0;
        events[curEventIdx].SetActive(true);
    }

    public void moveToNextEvent() {
        events[curEventIdx].SetActive(false);

        if (curEventIdx+1 >= events.Length){
            Debug.Log("Somehow exceeded the number of events.");
            return;
        }

        GameObject nextEvent = events[++curEventIdx];
        nextEvent.SetActive(true);
        Debug.Log("Next event: " + nextEvent);

        if (finishLine != null) {
            finishLine.transform.rotation = nextEvent.transform.rotation;
            Vector3 curPosition = finishLine.transform.position;
            finishLine.transform.position = new Vector3(curPosition.x, curPosition.y,
                                                    nextEvent.transform.position.z) + offsetFromEventDetect;
        }
    }

    public void moveToPreviousEvent() {
        events[curEventIdx].SetActive(false);
        if (curEventIdx-1 < 0) {
            Debug.Log("Already at first event the number of events.");

            return;
        }

        GameObject prevEvent = events[--curEventIdx];
        prevEvent.SetActive(true);
        Debug.Log("Prev event: " + prevEvent);

        // NOTE: finishLine code is leftover when EventSequence was simpler (See Basic_Multilane)
        //       since it won't be used there anymore, no need to implement here
    }
}
