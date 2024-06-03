using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothness;
    public Transform targetObject;
    public float shoulderLookDegree;
    private Vector3 initialOffset;
    private Quaternion initialRotate;
    private Vector3 cameraPosition;
    public Vector3 viewOffset;
    public Vector3 looBackOffset;

    void Start()
    {
        initialOffset = transform.position - targetObject.position;
        initialRotate = transform.rotation;
        viewOffset = initialOffset;
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(1)) {
            viewOffset = initialOffset + new Vector3(0, 0, 7);
            Vector3 curAngles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(Vector3.Lerp(curAngles, new Vector3(0,180+shoulderLookDegree,0), smoothness*Time.fixedDeltaTime));
        }
        if (Input.GetMouseButtonDown(0)) {
            viewOffset = initialOffset + new Vector3(0, 0, 7);
            Vector3 curAngles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(Vector3.Lerp(curAngles, new Vector3(0,-shoulderLookDegree,0), smoothness*Time.fixedDeltaTime));

        }

        if (Input.GetMouseButtonDown(2)) {
            viewOffset = initialOffset;
            transform.rotation = initialRotate;
        }

        cameraPosition = targetObject.position + viewOffset;
        transform.position = Vector3.Lerp(transform.position, cameraPosition, smoothness*Time.fixedDeltaTime);
        //     transform.LookAt(targetObject.position);
        // transform.rotation = targetObject.rotation;

        // if (targetObject.eulerAngles.x != transform.eulerAngles.x) {
        //     transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetObject.eulerAngles, smoothness*Time.fixedDeltaTime);
        //     transform.LookAt(targetObject.position);
        // }
    }
}
