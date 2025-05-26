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
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i) {
            Debug.Log(i);
            string sceneName = SceneUtility.GetScenePathByBuildIndex(i);
            int nameStart = sceneName.LastIndexOf("/") + 1;
            int nameEnd = sceneName.LastIndexOf(".unity");

            Debug.Log(sceneName + " => " + sceneName.Substring(nameStart, (nameEnd-nameStart)));
            sceneName = sceneName.Substring(nameStart, (nameEnd-nameStart));
            string sceneName_i = sceneName?.ToLower() ?? "";

            string stats = "["
                + new StringBuilder().Insert(0, "{\"AvgSpeed\": 1,\"ElapsedTime\": 2,\"Errors\": errors, \"ErrorsClassification\": [\"No Blinker\", \"Wrong Blinker\"]}", 20).ToString()
                + "]";
#if (!UNITY_EDITOR && UNITY_ANDROID)
            stats = Stats.GetStatsForScene(sceneName);
#endif

            string statsText = "\n" + sceneName + "\n";
            JSONNode statNodes = JSON.Parse(stats);
            foreach (JSONNode stat in statNodes) {
                Debug.Log(stat);
                Debug.Log(stat["ErrorsClassification"]);
                string[] errorsClassification = stat["ErrorsClassification"].ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string curStats = String.Join("\n\t", Stats.formatStats(stat["AvgSpeed"], stat["ElapsedTime"], stat["Errors"], errorsClassification));
                statsText += $"\t{curStats}\n\n";
                Debug.Log($"Stats for {sceneName}: {curStats}");
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
