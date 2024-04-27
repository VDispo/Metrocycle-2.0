using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedRoad : MonoBehaviour
{
    [SerializeField] private GameObject linkedRoad;

    public void setOtherRoad(GameObject other)
    {
        linkedRoad = other;
    }
}
