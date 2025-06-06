using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleJSON;
using TMPro;

public class ShowStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI motorText;
    [SerializeField] private TextMeshProUGUI bikeText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI bestErrorsText;
    [SerializeField] private TextMeshProUGUI numberOfPlaysText;
    private Dictionary<string, JSONNode> motorSceneStats;
    private Dictionary<string, JSONNode> bikeSceneStats;

    void Awake()
    {
        motorSceneStats = new Dictionary<string, JSONNode>();
        bikeSceneStats = new Dictionary<string, JSONNode>();

        string motorStats = "";
        string bikeStats = "";
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
        {
            Debug.Log(i);
            string sceneName = SceneUtility.GetScenePathByBuildIndex(i);
            int nameStart = sceneName.LastIndexOf("/") + 1;
            int nameEnd = sceneName.LastIndexOf(".unity");

            Debug.Log(sceneName + " => " + sceneName.Substring(nameStart, (nameEnd - nameStart)));
            sceneName = sceneName.Substring(nameStart, (nameEnd - nameStart));
            string sceneName_i = sceneName?.ToLower() ?? "";

            string stats = "["
                + new StringBuilder().Insert(0, "{\"AvgSpeed\": 1,\"ElapsedTime\": 2,\"Errors\": errors, \"ErrorsClassification\": [\"No Blinker\", \"Wrong Blinker\"]}", 20).ToString()
                + "]";

            #if !UNITY_EDITOR && UNITY_WEBGL
                stats = Stats.GetStatsForScene_JS(sceneName);
            #else
                stats = Stats.GetStatsForScene(sceneName);
            #endif

            Debug.Log($"Stats for {sceneName}: {JSONNode.Parse(stats)}");

            string statsText = "\n" + String.Join(" ", sceneName.ToString().Split("_")) + "\n";
            JSONNode statNodes = JSON.Parse(stats);
            foreach (JSONNode stat in statNodes["entries"])
            {

                string[] errorsClassification = stat["ErrorsClassification"].ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                int score = stat["Score"].AsInt;

                string curStats = String.Join("\n\t", Stats.GetStatsText(stat["AvgSpeed"], stat["ElapsedTime"], stat["Errors"], errorsClassification, score).Split('\n'));
                statsText += $"\t{curStats}\n\n";
                Debug.Log($"Stats for {sceneName}: {curStats}");
            }
            
            if (statNodes["entries"].Count == 0)
            {
                statsText += "\tNo stats available for this scene\n";
            }

            if (sceneName_i.EndsWith("motorcycle"))
            {
                Debug.Log("motor");
                motorSceneStats.Add(sceneName, statNodes);
                motorStats += statsText;
            }
            else if (sceneName_i.EndsWith("bicycle"))
            {
                Debug.Log("bike");
                bikeSceneStats.Add(sceneName, statNodes);
                bikeStats += statsText;
            }
        }

        motorText.text = motorStats;
        bikeText.text = bikeStats;

        ShowBestScore("Motorcycle");
    }

    public void ShowBestScore(string vehicle)
    {
        Debug.Log($"Showing best score for {vehicle}");
        int bestScore = PlayerPrefs.GetInt($"BestScore_{vehicle}", 0);
        float bestTime = PlayerPrefs.GetFloat($"BestTime_{vehicle}", 0f);
        int bestErrors = PlayerPrefs.GetInt($"BestErrors_{vehicle}", 0);
        bestScoreText.text = bestScore.ToString();
        int minutes = Mathf.FloorToInt(bestTime / 60f);
        int seconds = Mathf.FloorToInt(bestTime % 60f);
        bestTimeText.text = $"{minutes:00}:{seconds:00}";
        bestErrorsText.text = bestErrors.ToString();
        numberOfPlaysText.text = PlayerPrefs.GetInt($"TotalPlays_{vehicle}", 0).ToString();
    }
}
