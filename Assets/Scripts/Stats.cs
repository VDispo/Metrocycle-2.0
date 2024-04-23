using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

public class PlayerStats
{
    public float speed;
    public float timeToComplete;
    public int collisionCount;

    public PlayerStats(float speed, float timeToComplete, int collisionCount)
    {
        this.speed = speed;
        this.timeToComplete = timeToComplete;
        this.collisionCount = collisionCount;
    }
}

public class Stats : MonoBehaviour
{
    private static string filePath;

    public static void SaveSpeed(float speed, string sceneId)
    {
        filePath = "Assets/PlayerStats_" + sceneId + ".json";
        JSONNode userStatsJson = LoadUserStats(filePath);
        userStatsJson["speed"] = speed;
        SaveUserStats(userStatsJson, filePath);
    }

    public static void SaveTime(float time, string sceneId)
    {
        filePath = "Assets/PlayerStats_" + sceneId + ".json";
        JSONNode userStatsJson = LoadUserStats(filePath);
        userStatsJson["time"] = time;
        SaveUserStats(userStatsJson, filePath);
    }

    public static void SaveCollisionCount(int collisionCount, string sceneId)
    {
        filePath = "Assets/PlayerStats_" + sceneId + ".json";
        JSONNode userStatsJson = LoadUserStats(filePath);
        userStatsJson["collisionCount"] = collisionCount;
        SaveUserStats(userStatsJson, filePath);
    }

    public static void IncrementCollisionCount(string sceneId)
    {
        filePath = "Assets/PlayerStats_" + sceneId + ".json";
        JSONNode userStatsJson = LoadUserStats(filePath);
        userStatsJson["collisionCount"] = userStatsJson["collisionCount"].AsInt + 1;
        SaveUserStats(userStatsJson, filePath);
    }

    public static void SaveAllStats(float speed, float time, int collisionCount, string sceneId)
    {
        filePath = "Assets/PlayerStats_" + sceneId + ".json";
        JSONNode userStatsJson = new JSONObject();
        userStatsJson["speed"] = speed;
        userStatsJson["time"] = time;
        userStatsJson["collisionCount"] = collisionCount;
        SaveUserStats(userStatsJson, filePath);
    }

    public static JSONNode LoadUserStats(string filePath)
    {
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            return JSON.Parse(jsonString);
        }
        else
        {
            return new JSONObject();
        }
    }

    private static void SaveUserStats(JSONNode userStatsJson, string filePath)
    {
        File.WriteAllText(filePath, userStatsJson.ToString());
    }
}