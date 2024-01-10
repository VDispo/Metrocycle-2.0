using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FictionalRoadDetect : MonoBehaviour
{
    public GameObject fictionalRoadObject;
    public GameObject motorObject;
    public GameObject pairCollider;
    public int direction;

    void Start()
    {
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered collision with " + other.gameObject.name);

        fictionalRoadObject.SendMessage("nextRoad", direction, SendMessageOptions.DontRequireReceiver);
    }
}
