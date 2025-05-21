using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionalTesting : MonoBehaviour
{
    private static float CurrentFPS;
    private static float AvgFPS;
    private static float HighestFPS;
    private static float LowestFPS;

    private static float SumFPS;
    private static int numFPSSamples;
    public float deltaTime;

    void Start()
    {
        HighestFPS = float.MinValue;
        LowestFPS = float.MaxValue;

        SumFPS = 0;
        numFPSSamples = 0;
        deltaTime = 0.0001f; // Initial small progress to avoid division by zero
    }

    // Update is called once per frame
    void Update()
    {
        // If the game is NOT paused, measure FPS
        if (Time.timeScale != 0){
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            if (deltaTime > 0) {
                CurrentFPS = 1.0f / deltaTime;

                // Max FPS is 60
                if (CurrentFPS <= 60) {
                    // Record the lowest and highest FPS
                    if (CurrentFPS < LowestFPS) {
                        LowestFPS = CurrentFPS;
                    }
                    if (CurrentFPS > HighestFPS) {
                        HighestFPS = CurrentFPS;
                    }

                    SumFPS += CurrentFPS;
                    numFPSSamples += 1;
                    AvgFPS = SumFPS / numFPSSamples;

                    // Debug.Log($"FPS: {CurrentFPS} | AVGFPS: {AvgFPS} | LOWFPS: {LowestFPS} | HIGHFPS: {HighestFPS}");
                }
            }
        }
    }
}
