using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class Stats : MonoBehaviour
{
     [DllImport("__Internal")]
     public static extern void SaveStats(string sceneName, float speed, float elapsedTime, int errors, string[] errorsClassification);

     [DllImport("__Internal")]
     public static extern string GetStatsForScene(string sceneName);


     void Awake() {
          string sceneName = SceneManager.GetActiveScene().name;
          if(PlayerPrefs.HasKey(sceneName+"_Errors")){
          }
          else{
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
          string[] errorsClassification = PlayerPrefs.GetString(sceneName+"_ErrorsClassification").Split(',');

          Debug.Log("Speed: " + speed);
          Debug.Log("Elapsed Time: " + elapsedTime);
          Debug.Log("Errors: " + errors);
          Debug.Log("Errors Classification: " + string.Join(", ", errorsClassification));

          // Hello();
          return (speed, elapsedTime, errors);
     }

     public static string[] AddUserError(string errorClassification) {
          string sceneName = SceneManager.GetActiveScene().name;
          string errorsClassification = PlayerPrefs.GetString(sceneName+"_ErrorsClassification");

          if (errorsClassification == "") {
               errorsClassification = errorClassification;
          } else {
               errorsClassification += "," + errorClassification;
          }

          PlayerPrefs.SetString(sceneName+"_ErrorsClassification", errorsClassification);

          return errorsClassification.Split(',');
     }

     public static string[] formatStats(float speed, float elapsedTime, int errors) {
          string timeText;
          TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
          if (time.Seconds.ToString().Length == 1){
               timeText = "Time: " + time.Minutes.ToString() + ":0" + time.Seconds.ToString();
          }
          else {
               timeText = "Time: " + time.Minutes.ToString() + ":" + time.Seconds.ToString();
          }

          return new string[] {
               timeText,
               "Avg Speed: " + speed.ToString("F2") + " kph",
               "Errors: " + errors.ToString()
          };
     }
}
