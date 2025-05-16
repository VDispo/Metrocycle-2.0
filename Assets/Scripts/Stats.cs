using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.Linq;

public class Stats : MonoBehaviour
{
     [DllImport("__Internal")]
     public static extern void SaveStats(string sceneName, float speed, float elapsedTime, int errors, string[] errorsClassification);

     [DllImport("__Internal")]
     public static extern string GetStatsForScene(string sceneName);


     void Awake()
     {
          ResetStats();
     }

     public static void ResetStats()
     {
          string sceneName = SceneManager.GetActiveScene().name;
          PlayerPrefs.SetInt(sceneName + "_Errors", 0);
          PlayerPrefs.SetString(sceneName + "_ErrorsClassification", "");
     }

     public static void SetSpeed()
     {
          string sceneName = SceneManager.GetActiveScene().name;

          float Speed = Speedometer.GetAvgSpeed();
          PlayerPrefs.SetFloat(sceneName + "_Speed", Speed);
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

     public static (float speed, float elapsedTime, int errors, string[] errorsClassification) GetStats()
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
          return (speed, elapsedTime, errors, errorsClassification);
     }

     public static string[] AddUserError(string errorClassification) {
          string sceneName = SceneManager.GetActiveScene().name;
          string errorsClassification = PlayerPrefs.GetString(sceneName+"_ErrorsClassification");

          Debug.Log("Current Errors Classification: " + errorsClassification);

          if (errorsClassification == "") {
               errorsClassification = errorClassification;
          } else {
               errorsClassification += "," + errorClassification;
          }

          Debug.Log("After Errors Classification: " + errorsClassification);

          PlayerPrefs.SetString(sceneName+"_ErrorsClassification", errorsClassification);

          return errorsClassification.Split(',');
     }

     public static string[] formatStats(float speed, float elapsedTime, int errors, string[] errorsClassification = null) {
          string timeText;
          TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
          if (time.Seconds.ToString().Length == 1){
               timeText = time.Minutes.ToString() + ":0" + time.Seconds.ToString();
          }
          else {
               timeText = time.Minutes.ToString() + ":" + time.Seconds.ToString();
          }

          Dictionary<string, int> errorCounts = new Dictionary<string, int>();
          if (errorsClassification == null) {
               errorsClassification = new string[0];
          }
          foreach (var error in errorsClassification)
          {
               if (string.IsNullOrEmpty(error)) continue;
               if (errorCounts.ContainsKey(error))
                    errorCounts[error]++;
               else
                    errorCounts[error] = 1;
          }

          string groupedErrors = "";
          int mistakeNum = 1;
          foreach (var kvp in 
               new List<KeyValuePair<string, int>>(errorCounts)
                    .OrderByDescending(e => e.Value)
                    .ThenBy(e => e.Key))
          {
               groupedErrors += $"{kvp.Key}: committed {kvp.Value} time{(kvp.Value == 1 ? "" : "s")}\n\t";
               mistakeNum++;
          }

          if (groupedErrors.Length == 0)
          {
               groupedErrors = "None";
          }


          return new string[] {
               timeText,
               speed.ToString("F2") + " kph",
               errors.ToString(),
               groupedErrors
          };
     }
}
