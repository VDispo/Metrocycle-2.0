using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionLaneDetect : MonoBehaviour
{
    void OnTriggerEnter (Collider other) {
        transform.parent.parent.gameObject.SendMessage("laneDetectEntered", this.gameObject, SendMessageOptions.DontRequireReceiver);
        GameManager.Instance.startBlinkerCancelTimer();
    }
}
