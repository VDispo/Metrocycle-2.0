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
    // Start is called before the first frame update
    void Start()
    {
        floor1 = Instantiate(floorPrefab, new Vector3(0,0,0), Quaternion.identity);

        float floorZLength = floor1.GetComponent<Renderer>().bounds.size.z;
        for (int i = 0; i < bufferRoadCount; ++i) {
            Instantiate(floorPrefab, new Vector3(0,0,floorZLength*(i+1)), Quaternion.identity);
            Instantiate(floorPrefab, new Vector3(0,0,-floorZLength*(i+1)), Quaternion.identity);
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
}
