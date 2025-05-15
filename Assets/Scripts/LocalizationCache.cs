using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationCache : MonoBehaviour
{
    /// <summary>
    /// This script is meant to stay alive for the whole duration of the game to maximize caches.
    /// Hence, it is in the Start Scene and is in DontDestroyOnLoad.
    /// 
    /// However, during development, we don't necessarily play from the Start Scene, this script
    /// can be put in individual scenes, and this instance would be the one to live for the whole 
    /// duration for the game.
    /// </summary>

    public static LocalizationCache Instance;
    private Dictionary<string, string> cache = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Very useful for scripts that are referenced multiple times but have the same constant string to reference
    /// (especially for SpeedChecker.cs since it is in more than 5000 elements just in the Commonwealth scene)
    /// </summary>
    /// <param name="table"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetLocalizedString(string table, string key)
    {
        // Try to find it in this cache then return
        if (cache.TryGetValue(key, out string result)) return result;

        // Fetch synchronously from the Localization System
        var tableRef = LocalizationSettings.StringDatabase.GetTable(table);
        if (tableRef != null)
        {
            var entry = tableRef.GetEntry(key);
            Debug.Log($"Entry: {entry}");
            if (entry != null)
            {
                result = entry.GetLocalizedString();
                cache[key] = result; // Cache the value
                return result;
            }
        }

        // If not found, return key as fallback
        return key; 
    }
}
