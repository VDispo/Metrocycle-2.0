using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFinishLineToNextEvent : MonoBehaviour
{
    public GameObject finishLine;
    [SerializeField]
    public GameObject[] events;
    public Vector3 offsetFromEventDetect;

    private int curEventIdx;

    // Start is called before the first frame update
    void Start()
    {
        curEventIdx = 0;
    }

    public void moveToNextEvent() {
        if (++curEventIdx >= events.Length){
            Debug.Log("Somehow exceeded the number of events.");
            return;
        }

        GameObject nextEvent = events[curEventIdx];
        finishLine.transform.rotation = nextEvent.transform.rotation;
        Vector3 curPosition = finishLine.transform.position;
        finishLine.transform.position = new Vector3(curPosition.x, curPosition.y,
                                                    nextEvent.transform.position.z) + offsetFromEventDetect;
    }
}
