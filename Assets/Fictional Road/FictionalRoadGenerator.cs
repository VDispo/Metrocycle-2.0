using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FictionalRoadGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject frontColliderPrefab;
    public GameObject backColliderPrefab;
    public GameObject motorObject;
    public GameObject gameoverPopup;
    public GameObject finishLine;

    // TODO: obstacle generator should be its own class
    public GameObject obstaclePrefab;
    public int maxObstaclesPerRoad;

    // number of roads before and after current vehicle position which are loaded
    // e.g. a value of 3 means 3 road units on front and back of the vehicle
    public int bufferRoadCount;
    public Vector3 frontColliderOffset;
    public Vector3 backColliderOffset;

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

        Vector3 offset = floor1.transform.position + frontColliderOffset;
        frontCollider = Instantiate(frontColliderPrefab, offset, frontColliderPrefab.transform.rotation);

        offset = floor1.transform.position + backColliderOffset;
        backCollider = Instantiate(backColliderPrefab, offset, backColliderPrefab.transform.rotation);

        foreach (GameObject collider in new [] {frontCollider, backCollider}) {
            FictionalRoadDetect collideDetect = collider.GetComponent<FictionalRoadDetect>();
            collideDetect.fictionalRoadObject = this.gameObject;
            collideDetect.motorObject = motorObject;

            collideDetect.pairCollider = collider == frontCollider ? backCollider : frontCollider;
        }
    }

    private GameObject createForwardRoad() {
        return createRoad(1);
    }

    private GameObject createBackwardRoad() {
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

        if (direction == 1) {
            roadQueue.AddLast(newRoad);
            generateObstacles(newRoad);
        } else {
            roadQueue.AddFirst(newRoad);
        }

        return newRoad;
    }

    public GameObject nextRoad(int direction) {
        Debug.Log("nextRoad " + direction);
        // ASSUMPTION: direction 1 = Forward, -1 = Backward (no checking done for now)
        // Note: index starts at 0, so bufferRoadCount was the middle (=current) road segment
        // Since we just passed a collider, the adjacent road is NOW the current road segment
        GameObject curRoad = roadQueue[bufferRoadCount+direction];

        // move colliders
        Vector3 curRoadHalfLength = new Vector3(0, 0, curRoad.GetComponent<Renderer>().bounds.size.z);
        frontCollider.transform.position = curRoad.transform.position + curRoadHalfLength + frontColliderOffset;
        backCollider.transform.position = curRoad.transform.position - curRoadHalfLength - backColliderOffset;

        GameObject newRoad = createRoad(direction);

        // Remove road not in buffer zone
        if (direction == 1)
            Destroy(roadQueue.PopFirst());
        else
            Destroy(roadQueue.PopLast());

        // add finish line
        float chance = Random.Range(0f, 1f);
        Debug.Log("chance: " + chance);
        if (direction == 1 && (!finishLine.activeSelf) && chance <= 0.04) {
            finishLine.transform.SetParent(newRoad.transform, false);
            finishLine.SetActive(true);
        }

        return newRoad;
    }

    private void generateObstacles(GameObject road)
    {
        Vector3 roadDimenions = road.GetComponent<Renderer>().bounds.size;

        int numObstacles = Random.Range(0, maxObstaclesPerRoad);
        // NOTE: 1 unit offset in y axis to ensure obstacle is above floor
        Vector3 offset = new Vector3(0, 1, 0);
        GameObject obstacle;
        for (int i = 0; i < numObstacles; ++i) {
            // Randomize position in road segment
            // TODO: add more randomization (e.g. length, rotation)
            // TODO: ensure that obstacles leave at least one path open
            // TODO: (not too high priority) ensure that obstacles don't overlap
            offset.x = Random.Range(-roadDimenions.x/2, roadDimenions.x/2);
            offset.z = Random.Range(-roadDimenions.z/2, roadDimenions.z/2);

            obstacle = Instantiate(obstaclePrefab, road.transform.position + offset, Quaternion.identity);
            obstacle.GetComponent<ObstacleCollide>().gameoverPopup = gameoverPopup;
            obstacle.transform.SetParent(road.transform, true);
        }
    }
}
