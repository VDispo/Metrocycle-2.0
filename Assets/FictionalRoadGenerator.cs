using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FictionalRoadGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject frontColliderPrefab;
    public GameObject backColliderPrefab;
    public GameObject motorObject;

    // number of roads before and after current vehicle position which are loaded
    // e.g. a value of 3 means 3 road units on front and back of the vehicle
    public int bufferRoadCount;
    public Vector3 frontColliderOffset;
    public Vector3 backColliderOffset;

    public Vector3 offset;

    private GameObject floor1;
    private GameObject frontCollider;
    private GameObject backCollider;

    private Deque<GameObject> roadQueue;
    // Start is called before the first frame update
    void Start()
    {
        roadQueue = new Deque<GameObject>(bufferRoadCount*2 + 1);
        floor1 = createForwardRoad();

        for (int i = 0; i < bufferRoadCount; ++i) {
            createForwardRoad();
            createBackwardRoad();
        }

        offset = floor1.transform.position + frontColliderOffset;
        frontCollider = Instantiate(frontColliderPrefab, offset, frontColliderPrefab.transform.rotation);

        offset = floor1.transform.position + backColliderOffset;
        backCollider = Instantiate(backColliderPrefab, offset, backColliderPrefab.transform.rotation);

        foreach (GameObject collider in new [] {frontCollider, backCollider}) {
            RoadMover roadMover = collider.GetComponent<RoadMover>();
            roadMover.objectToMove = floor1;
            roadMover.motorObject = motorObject;

            roadMover.pairCollider = collider == frontCollider ? backCollider : frontCollider;
        }
    }

    GameObject createForwardRoad() {
        return createRoad(1);
    }

    GameObject createBackwardRoad() {
        return createRoad(-1);
    }

    private GameObject createRoad(int direction) {
        // ASSUMPTION: direction 1 = Forward, -1 = Backward (no checking done for now)
        Vector3 position = new Vector3(0, 0, 0);
        GameObject newRoad = Instantiate(floorPrefab, position, Quaternion.identity);

        if (!roadQueue.IsEmpty) {
            GameObject lastRoad = (direction == 1) ? roadQueue.PeekLast() : roadQueue.PeekFirst();
            float lastRoadLength = lastRoad.GetComponent<Renderer>().bounds.size.z;
            float newRoadStart = lastRoad.transform.position.z + direction*(lastRoadLength/2);

            position.z = newRoadStart + direction*(newRoad.GetComponent<Renderer>().bounds.size.z/2);
            newRoad.transform.position = position;
        }

        if (direction == 1)
            roadQueue.AddLast(newRoad);
        else
            roadQueue.AddFirst(newRoad);

        return newRoad;
    }
}
