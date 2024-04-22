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
    private string filePath = "Assets/PlayerStats.json";

    public static void SaveSpeed(float speed)
    {
        JSONNode userStatsJson = LoadUserStats();
        userStatsJson["speed"] = speed;
        SaveUserStats(userStatsJson);
    }

    public static void SaveTime(float time)
    {
        JSONNode userStatsJson = LoadUserStats();
        userStatsJson["time"] = time;
        SaveUserStats(userStatsJson);
    }

    public static void SaveCollisionCount(int collisionCount)
    {
        JSONNode userStatsJson = LoadUserStats();
        userStatsJson["collisionCount"] = collisionCount;
        SaveUserStats(userStatsJson);
    }

    public static void IncrementCollisionCount()
    {
        JSONNode userStatsJson = LoadUserStats();
        userStatsJson["collisionCount"] = userStatsJson["collisionCount"]+1;
        SaveUserStats(userStatsJson);
    }

    public static void SaveAllStats(float speed, float time, int collisionCount)
    {
        JSONNode userStatsJson = new JSONObject();
        userStatsJson["speed"] = speed;
        userStatsJson["time"] = time;
        userStatsJson["collisionCount"] = collisionCount;
        SaveUserStats(userStatsJson);
    }

    public static JSONNode LoadUserStats()
    {
        // Check if the JSON file exists
        if (File.Exists(filePath))
        {
            // Read the JSON file as a string
            string jsonString = File.ReadAllText(filePath);

            // Parse the JSON string into a JSONNode
            return JSON.Parse(jsonString);
        }
        else
        {
            // Create a new JSONObject if the file doesn't exist
            return new JSONObject();
        }
    }

    private void SaveUserStats(JSONNode userStatsJson)
    {
        // Write the JSONObject to a JSON file
        File.WriteAllText(filePath, userStatsJson.ToString());
    }

    void Start()
    {
        // Example usage
        SaveSpeed(10.0f);
        SaveTime(120.0f);
        SaveCollisionCount(5);
        IncrementCollisionCount();

        // Load and display user stats
        JSONNode userStatsJson = LoadUserStats();
        float speed = userStatsJson["speed"].AsFloat;
        float time = userStatsJson["time"].AsFloat;
        int collisionCount = userStatsJson["collisionCount"].AsInt;

        Debug.Log("Speed: " + speed);
        Debug.Log("Time: " + time);
        Debug.Log("Collision Count: " + collisionCount);
    }
}
