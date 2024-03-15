using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;

public class CollisionWithObstacles : MonoBehaviour
{
    public GameObject gameoverPopup;
    private TextMeshProUGUI objectName;

    // Start is called before the first frame update
    void Start()
    {
        objectName = gameoverPopup.transform.Find("Instructions").GetComponent<TextMeshProUGUI>();
    }

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
        switch (otherLayer) {
            case layer_AITraffic:   otherDescription = "another vehicle"; break;
            case layer_obstacles:   otherDescription = "the side of the road"; break;
            default:                break;
        }

        objectName.text = "You collided with " + otherDescription + ". Remember to control your speed and direction.";
        gameoverPopup.SetActive(true);
        gameoverPopup.SendMessage("popupShown", null, SendMessageOptions.DontRequireReceiver);
        Time.timeScale = 0;
    }

    public void restartGame() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
