using AYellowpaper.SerializedCollections;
using UnityEngine;

/// <summary>
/// Note that the name of the prefab (from <see cref="choicesPrefabs"/>) is the text used as display.
/// </summary>
[CreateAssetMenu(fileName = "CustomizationAssetSetSO", menuName = "CustomizationAssetSetSO")]
public class CustomizationAssetSetSO : ScriptableObject
{
    [Header("Data")]

    [Tooltip("defined in Constants.cs")] 
    public Metrocycle.CustomizationAssetType type;

    [Tooltip("the assets to choose from")][SerializedDictionary("Prefab", "Valid")] 
    public SerializedDictionary<GameObject, bool> choicesPrefabsWithPassing;
}
