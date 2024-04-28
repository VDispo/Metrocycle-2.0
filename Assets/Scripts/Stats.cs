using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stats : MonoBehaviour
{
   public static void SetStats (string SceneName, float Speed, float elapsedTime, int Errors)
   {
        string sceneName = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetFloat(SceneName+"_Speed", Speed);
        PlayerPrefs.SetFloat(SceneName+"_elapsedTime", elapsedTime);
        PlayerPrefs.SetInt(SceneName+"_Errors", Errors);
   }

    public static void GetStats (string SceneName)
   {
        string sceneName = SceneManager.GetActiveScene().name;

        Debug.Log(PlayerPrefs.GetFloat(SceneName+"_Speed"));
        Debug.Log(PlayerPrefs.GetFloat(SceneName+"_elapsedTime"));
        Debug.Log(PlayerPrefs.GetInt(SceneName+"_Errors"));
        Debug.Log("TEST2");
   }
}