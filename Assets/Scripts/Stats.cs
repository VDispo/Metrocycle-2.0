using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stats : MonoBehaviour
{
     void Awake() {
          string sceneName = SceneManager.GetActiveScene().name;
          if (PlayerPrefs.GetInt(sceneName+"_Errors")==0){
               SetErrors(0);
          }
    }

     public static void SetSpeed ()
     {
          string sceneName = SceneManager.GetActiveScene().name;

          float Speed = Speedometer.GetAvgSpeed();
          PlayerPrefs.SetFloat(sceneName+"_Speed",Speed);
     }

     public static void SetTime ()
     {
          string sceneName = SceneManager.GetActiveScene().name;

          float elapsedTime = Timer.CurrentTime;
          PlayerPrefs.SetFloat(sceneName+"_elapsedTime", elapsedTime);
     }

     public static void SetErrors (int Errors)
     {
          string sceneName = SceneManager.GetActiveScene().name;

          PlayerPrefs.SetInt(sceneName+"_Errors", Errors);
     }

     public static void incrementErrors ()
     {
          string sceneName = SceneManager.GetActiveScene().name;
          int errors = PlayerPrefs.GetInt(sceneName+"_Errors");

          PlayerPrefs.SetInt(sceneName+"_Errors", errors+1);
     }

     public static (float speed, float elapsedTime, int errors) GetStats()
     {
          string sceneName = SceneManager.GetActiveScene().name;

          float speed = PlayerPrefs.GetFloat(sceneName+"_Speed");
          float elapsedTime = PlayerPrefs.GetFloat(sceneName+"_elapsedTime");
          int errors = PlayerPrefs.GetInt(sceneName+"_Errors");

          return (speed, elapsedTime, errors);
     }
}