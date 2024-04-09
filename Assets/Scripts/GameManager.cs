using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* To use, attach this script to the "Bike" prefab instance
 */

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public popUp PopupSystem = null;

    public GameObject bike;
    private Rigidbody bikeRB;

    private GameObject bikeTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (PopupSystem == null) {
            PopupSystem = GameObject.Find("/popUp").GetComponent<popUp>();
            Debug.Log("PopupSystem Force Initialized " + PopupSystem);
        }

        setBikeType(Metrocycle.BikeType.Motorcycle);    // TODO: move this call to selection of motorcycle or bicycle
        bikeTransform = gameObject.transform.GetChild(2).gameObject;
    }

    public GameObject setBikeType(Metrocycle.BikeType type)
    {
        // disable previous bike
        if (bike != null) {
            Debug.LogError("Bicycle type can only be set once per Scene");
            return bike;
        }

        switch(type) {
            case Metrocycle.BikeType.Bicycle:
                bike = gameObject.transform.GetChild(1).gameObject;
                Debug.Assert(bike.name == "Bicycle");
                break;
            default:
            case Metrocycle.BikeType.Motorcycle:
                bike = gameObject.transform.GetChild(0).gameObject;
                Debug.Assert(bike.name == "Motorcycle");
                break;
        }

        bikeRB = bike.GetComponent<Rigidbody>();

        bike.SetActive(true);
        return bike;
    }

    public float getBikeSpeed()
    {
        return bikeRB.velocity.magnitude * 3;     // HACK: *3 is just based on "feel" for now
    }

    public GameObject getBlinkers()
    {
        GameObject dashboardCanvas = gameObject.transform.GetChild(3).gameObject;
        Debug.Assert(dashboardCanvas.name == "Dashboard Canvas");
        GameObject blinkers = dashboardCanvas.transform.GetChild(1).gameObject;
        Debug.Assert(blinkers.name == "Blinkers");

        return blinkers;
    }

    public void pauseGame() {
        Time.timeScale = 0;
    }
    public void resumeGame() {
        Time.timeScale = 1;
    }
    public void restartGame() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update() {
        if (bike == null) {
            return;
        }

        bikeTransform.transform.position = bike.transform.position;
        bikeTransform.transform.rotation = bike.transform.rotation;
        bikeTransform.transform.localScale = bike.transform.localScale;
    }
}
