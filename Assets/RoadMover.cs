using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadMover : MonoBehaviour
{
    public GameObject objectToMove;
    public GameObject motorObject;
    public GameObject pairCollider;
    public int direction;

    private Vector3 initialOffset;
    private Vector3 initialPairOffset;

    void Start()
    {
        initialOffset = transform.position - objectToMove.transform.position;
        initialPairOffset = pairCollider.transform.position - objectToMove.transform.position;
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered collision with " + other.gameObject.name);
        // motorObject.transform.position = transform.position;
        // objectToMove.transform.position = motorObject.transform.position;
        Vector3 origLocation = objectToMove.transform.localPosition;
        Vector3 newPosition = origLocation;
        newPosition.z = motorObject.transform.position.z + direction*(objectToMove.GetComponent<Collider>().bounds.size.z*2/6);

        objectToMove.transform.position = newPosition;
        transform.position = objectToMove.transform.position + initialOffset;
        pairCollider.transform.position = objectToMove.transform.position + initialPairOffset;
    }
}
