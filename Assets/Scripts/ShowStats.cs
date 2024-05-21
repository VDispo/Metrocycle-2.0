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
    private Dictionary<string, JSONNode> motorSceneStats;
    private Dictionary<string, JSONNode> bikeSceneStats;

    void Awake()
    {
        motorSceneStats = new Dictionary<string, JSONNode>();
        bikeSceneStats = new Dictionary<string, JSONNode>();

        string motorStats = "";
        string bikeStats = "";
        Debug.Log(SceneManager.sceneCount);
        for (int i = 0; i < SceneManager.sceneCount; ++i) {
            Debug.Log(i);
            string sceneName = SceneManager.GetSceneAt(i).name;
            string sceneName_i = sceneName.ToLower();

            string stats = "["
                + new StringBuilder().Insert(0, "{\"AvgSpeed\": 1,\"ElapsedTime\": 2,\"Errors\": errors,}", 20).ToString()
                + "]";
            #if (!UNITY_EDITOR && UNITY_WEBGL)
            stats = Stats.GetStatsForScene(sceneName);
            #endif

            string statsText = "\n" + sceneName + "\n";
            JSONNode statNodes = JSON.Parse(stats);
            foreach (JSONNode stat in statNodes) {
                statsText += "\t" + String.Join("\n\t", Stats.formatStats(stat["AvgSpeed"], stat["ElapsedTime"], stat["Errors"])) + "\n\n";
            }

            if (sceneName_i.EndsWith("motorcycle")) {
                Debug.Log("motor");
                motorSceneStats.Add(sceneName, statNodes);
                motorStats += statsText;
            } else if (sceneName_i.EndsWith("bicycle")) {
                Debug.Log("bike");
                bikeSceneStats.Add(sceneName, statNodes);
                bikeStats += statsText;
            }
        }

        motorText.text = motorStats;
        bikeText.text = bikeStats;
    }
}
