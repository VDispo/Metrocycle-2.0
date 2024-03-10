using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionWithOtherCar : MonoBehaviour
{
    public GameObject gameoverPopup;

    void OnCollisionEnter (Collision other)
    {
        // HACK: hardcode collision to only AI cars and obstacles for now
        //       may be use a layer mask parameter?
        const int layer_AITraffic = 9;
        const int layer_obstacles = 10;
        if (!(other.gameObject.layer == layer_AITraffic
            || other.gameObject.layer == layer_obstacles)
        ) {
            return;
        }

        Debug.Log("Obstacle hit by Layer: " + other.gameObject.layer + other.gameObject.name);
        gameoverPopup.SetActive(true);
        gameoverPopup.SendMessage("popupShown", null, SendMessageOptions.DontRequireReceiver);
        Time.timeScale = 0;
    }

    public void restartGame() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
