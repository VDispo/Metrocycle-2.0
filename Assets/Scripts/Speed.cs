using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    public GameObject bike;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = bike.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = rb.velocity;
        Debug.Log(vel);
    }
}
