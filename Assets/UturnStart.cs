using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UturnStart : MonoBehaviour
{
    public GameObject finishCollider;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered collision with " + other.gameObject.name);
        finishCollider.SetActive(true);
        gameObject.SetActive(false);
    }
}
