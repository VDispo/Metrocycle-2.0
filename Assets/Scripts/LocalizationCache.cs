using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationCache : MonoBehaviour
{
    public static LocalizationCache Instance;
    private Dictionary<string, string> cache = new();

    private void Awake()
    {
        if (Instance != null) return;
        
        Instance = this;
        DontDestroyOnLoad(this);
    }

    // Very useful for scripts that are referenced multiple times but have the same constant string to reference
    // (e.g., for SpeedChecker.cs since it is in more than 5k elements just in the Commonwealth scene)
    public string GetLocalizedString(string table, string key)
    {
        // Try to find it in this cache then return
        if (cache.TryGetValue(key, out string result)) return result;

        // Fetch synchronously from the Localization System
        var tableRef = LocalizationSettings.StringDatabase.GetTable(table);
        if (tableRef != null)
        {
            var entry = tableRef.GetEntry(key);
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
