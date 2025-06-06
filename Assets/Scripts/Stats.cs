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
     public static extern void SaveStats_JS(string sceneName, float speed, float elapsedTime, int errors, string[] errorsClassification, int score);

     [DllImport("__Internal")]
     public static extern string GetStatsForScene_JS(string sceneName);

     [Serializable]
     public class StatsEntry
     {
          public float AvgSpeed;
          public float ElapsedTime;
          public int Errors;
          public string[] ErrorsClassification;
          public int Score;
     }

     [Serializable]
     public class StatsEntryList
     {
          public List<StatsEntry> entries = new List<StatsEntry>();
     }


     void Awake()
     {
          ResetStats();
     }

     public static void SaveStats(string scenename, float speed, float elapsedTime, int errors, string[] errorsClassification, int score)
     {
          string existingJson = PlayerPrefs.GetString(scenename, string.Empty);
          StatsEntryList wrapper;
          if (string.IsNullOrEmpty(existingJson))
          {
               wrapper = new StatsEntryList();
          }
          else
          {
               // Deserialize existing JSON into the wrapper; if malformed, reset.
               try
               {
                    wrapper = JsonUtility.FromJson<StatsEntryList>(existingJson) ?? new StatsEntryList();
               }
               catch (Exception)
               {
                    wrapper = new StatsEntryList();
               }
          }

          // 2) Construct a new StatsEntry
          StatsEntry newEntry = new StatsEntry
          {
               AvgSpeed = speed,
               ElapsedTime = elapsedTime,
               Errors = errors,
               ErrorsClassification = errorsClassification ?? new string[0],
               Score = score
          };

          // 3) Append to the list and re‐serialize
          wrapper.entries.Add(newEntry);
          string updatedJson = JsonUtility.ToJson(wrapper);

          // 4) Persist via PlayerPrefs
          PlayerPrefs.SetString(scenename, updatedJson);
          PlayerPrefs.Save();
     }

     public static string GetStatsForScene(string scenename)
     {
          string existingJson = PlayerPrefs.GetString(scenename, string.Empty);
          return string.IsNullOrEmpty(existingJson) ? "[]" : existingJson;
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

     public static void SetTime()
     {
          string sceneName = SceneManager.GetActiveScene().name;

          float elapsedTime = Timer.CurrentTime;
          PlayerPrefs.SetFloat(sceneName + "_elapsedTime", elapsedTime);
     }

     public static void SetErrors(int Errors)
     {
          string sceneName = SceneManager.GetActiveScene().name;

          PlayerPrefs.SetInt(sceneName + "_Errors", Errors);
     }

     public static void incrementErrors()
     {
          string sceneName = SceneManager.GetActiveScene().name;
          int errors = PlayerPrefs.GetInt(sceneName + "_Errors");

          PlayerPrefs.SetInt(sceneName + "_Errors", errors + 1);
     }

     public static (float speed, float elapsedTime, int errors, string[] errorsClassification) GetStats()
     {
          string sceneName = SceneManager.GetActiveScene().name;

          float speed = PlayerPrefs.GetFloat(sceneName + "_Speed");
          float elapsedTime = PlayerPrefs.GetFloat(sceneName + "_elapsedTime");
          int errors = PlayerPrefs.GetInt(sceneName + "_Errors");
          string[] errorsClassification = PlayerPrefs.GetString(sceneName + "_ErrorsClassification").Split(',');

          Debug.Log("Speed: " + speed);
          Debug.Log("Elapsed Time: " + elapsedTime);
          Debug.Log("Errors: " + errors);
          Debug.Log("Errors Classification: " + string.Join(", ", errorsClassification));

          // Hello();
          return (speed, elapsedTime, errors, errorsClassification);
     }

     public static string[] AddUserError(string errorClassification)
     {
          string sceneName = SceneManager.GetActiveScene().name;
          string errorsClassification = PlayerPrefs.GetString(sceneName + "_ErrorsClassification");

          Debug.Log("Current Errors Classification: " + errorsClassification);

          if (errorsClassification == "")
          {
               errorsClassification = errorClassification;
          }
          else
          {
               errorsClassification += "," + errorClassification;
          }

          Debug.Log("After Errors Classification: " + errorsClassification);

          PlayerPrefs.SetString(sceneName + "_ErrorsClassification", errorsClassification);

          return errorsClassification.Split(',');
     }

     public static string[] formatStats(float speed, float elapsedTime, int errors, string[] errorsClassification = null)
     {
          string timeText;
          TimeSpan time = TimeSpan.FromSeconds(elapsedTime);
          if (time.Seconds.ToString().Length == 1)
          {
               timeText = time.Minutes.ToString() + ":0" + time.Seconds.ToString();
          }
          else
          {
               timeText = time.Minutes.ToString() + ":" + time.Seconds.ToString();
          }

          Dictionary<string, int> errorCounts = new Dictionary<string, int>();
          if (errorsClassification == null)
          {
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

     public static string GetStatsText(float speed, float elapsedTime, int errors, string[] errorsClassification = null, int score = 0)
     {
          string[] stats = formatStats(speed, elapsedTime, errors, errorsClassification);
          return $"Time: {stats[0]}\nSpeed: {stats[1]}\nScore: {score}\nErrors: {stats[2]}\nErrors Committed:\n\t{stats[3]}";
     }

     public static void UpdateLeaderboard(int score, float time, int errors, string vehicleType)
     {
          int bestScore = PlayerPrefs.GetInt($"BestScore_{vehicleType}", 0);
          if (score > bestScore)
          {
               PlayerPrefs.SetInt($"BestScore_{vehicleType}", score);
          }
          if (time < PlayerPrefs.GetFloat($"BestTime_{vehicleType}", Mathf.Infinity))
          {
               PlayerPrefs.SetFloat($"BestTime_{vehicleType}", time);
          }
          if (errors < PlayerPrefs.GetInt($"BestErrors_{vehicleType}", int.MaxValue))
          {
               PlayerPrefs.SetInt($"BestErrors_{vehicleType}", errors);
          }
          int playsSoFar = PlayerPrefs.GetInt($"TotalPlays_{vehicleType}", 0);
          PlayerPrefs.SetInt($"TotalPlays_{vehicleType}", playsSoFar + 1);
     }
}
