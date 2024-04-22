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

    public void SavePlayerStats(float speed, float timeToComplete, int collisionCount)
    {
        // Create a JSONObject to hold the player's stats
        JSONObject playerStatsJson = new JSONObject();
        playerStatsJson["speed"] = speed;
        playerStatsJson["timeToComplete"] = timeToComplete;
        playerStatsJson["collisionCount"] = collisionCount;

        // Write the JSONObject to a JSON file
        File.WriteAllText(filePath, playerStatsJson.ToString());
    }

    public PlayerStats LoadPlayerStats()
    {
        // Read the JSON file as a string
        string jsonString = File.ReadAllText(filePath);

        // Parse the JSON string into a JSONNode
        JSONNode playerStatsJson = JSON.Parse(jsonString);

        // Retrieve the player's stats from the JSONNode
        float speed = playerStatsJson["speed"].AsFloat;
        float timeToComplete = playerStatsJson["timeToComplete"].AsFloat;
        int collisionCount = playerStatsJson["collisionCount"].AsInt;

        // Return the player's stats as a PlayerStats object
        return new PlayerStats(speed, timeToComplete, collisionCount);
    }

    void Start()
    {
        SavePlayerStats(10.0f, 120.0f, 5);

        PlayerStats playerStats = LoadPlayerStats();
        Debug.Log("Speed: " + playerStats.speed);
        Debug.Log("Time to Complete: " + playerStats.timeToComplete);
        Debug.Log("Collision Count: " + playerStats.collisionCount);
    }
}
