using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stats : MonoBehaviour
{
   public static void SetStats (float Speed, float elapsedTime, int Errors)
   {
        string sceneName = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetFloat(sceneName+"_Speed", Speed);
        PlayerPrefs.SetFloat(sceneName+"_elapsedTime", elapsedTime);
        PlayerPrefs.SetInt(sceneName+"_Errors", Errors);
   }

    public static void SetSpeed (float Speed)
   {
        string sceneName = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetFloat(sceneName+"_Speed", Speed);
   }

   public static void SetTime (float elapsedTime)
   {
        string sceneName = SceneManager.GetActiveScene().name;

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


    public static void GetStats ()
   {
        string sceneName = SceneManager.GetActiveScene().name;
        
        float speed = PlayerPrefs.GetFloat(sceneName+"_Speed");
        float elapsedTime = PlayerPrefs.GetFloat(sceneName+"_elapsedTime");
        int errors = PlayerPrefs.GetInt(sceneName+"_Errors");

        Debug.Log(sceneName);
        Debug.Log(speed);
        Debug.Log(elapsedTime);
        Debug.Log(errors);

   }
}