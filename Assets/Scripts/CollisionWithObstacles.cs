using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;

public class CollisionWithObstacles : MonoBehaviour
{
    public PopupType popupType = PopupType.ERROR;

    void OnCollisionEnter (Collision other)
    {
        // HACK: hardcode collision to only AI cars and obstacles for now
        //       may be use a layer mask parameter?
        const int layer_AITraffic = 9;
        const int layer_obstacles = 10;

        int otherLayer = other.gameObject.layer;
        if (!(otherLayer == layer_AITraffic
            || otherLayer == layer_obstacles)
        ) {
            return;
        }

        Debug.Log("Obstacle hit by Layer: " + other.gameObject.layer + other.gameObject.name);

        string otherDescription = "";
        bool isVehicle = false;
        switch (otherLayer) {
            case layer_AITraffic:   otherDescription = "another vehicle"; isVehicle = true; break;
            case layer_obstacles:
                string tag = other.gameObject.tag;
                Debug.Log("TAG: " + tag);
                if (tag == "Side Obstacle") otherDescription = "an obstacle on the side of the road";
                else otherDescription = "the side of the road";
                break;
            default:                break;
        }

        GameManager.Instance.PopupSystem.popWithType(popupType,
            "Uh oh!", "You collided with " + otherDescription + ". Remember to control your speed and direction."
                + (isVehicle ? "When driving, maintain a minimum of 1 car length away from other vehicles." : "")
        );
    }
}
