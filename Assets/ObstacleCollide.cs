using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollide : MonoBehaviour
{
    void OnTriggerEnter (Collider other)
    {
        Debug.Log("Obstacle hit by" + other.gameObject.name);
    }
}
