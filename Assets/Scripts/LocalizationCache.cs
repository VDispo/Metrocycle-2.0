using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// This script is meant to stay alive for the whole duration of the game to maximize caches.
/// Hence, it is in the Start Scene and goes into DontDestroyOnLoad.<br/><br/>
/// 
/// However, during development, we don't necessarily always play from the Start Scene, 
/// hence this script is put in each individual scene, without worrying about duplicates 
/// since only the first instance will live for the whole duration for the game.
/// </summary>
// TODO: make this a completely static class (does not derive from MonoBehavior), see CustomSceneManager, 
// but not even as a wrapper for a singleton like that, just a static class on its own!
public class LocalizationCache : MonoBehaviour
{
    public static LocalizationCache Instance;
    private Dictionary<string, string> cache = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            gameObject.SetActive(true);
        }
        else Destroy(gameObject); // no duplicates, no reset
    }

    /// <summary>
    /// This function retrieves a string reference. If this is the first time for this table-key pair, it is fetched 
    /// directly from the localization table and then cached here. If not (i.e., it exists in the cache), simply fetch from this cache.
    /// <br/><br/>
    /// 
    /// Very useful for scripts that are referenced multiple times but have the same constant string to reference
    /// (especially for SpeedChecker.cs since it is in more than 5000 elements just in the Commonwealth scene)
    /// </summary>
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
                cache[key] = result; // Cache the progress
                return result;
            }
        }

        // If not found, return key as fallback
        return key; 
    }
}
