using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UturnStart : MonoBehaviour
{
    public bool hasPairCollider;
    public GameObject pairCollider; // activated after collision

    public bool deactivateAfterCollision;

    public bool activatePopup;
    public GameObject popup; // activated after collision


    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("Entered collision with " + other.gameObject.name);
        if (hasPairCollider) {
            pairCollider.SetActive(true);
        }

        if (deactivateAfterCollision) {
            gameObject.SetActive(false);
        }

        if (activatePopup) {
            popup.SetActive(true);
        }
    }
}
