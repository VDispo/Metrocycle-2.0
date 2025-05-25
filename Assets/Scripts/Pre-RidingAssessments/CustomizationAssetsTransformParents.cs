using UnityEngine;
using Metrocycle;
using AYellowpaper.SerializedCollections;
using System;

/// <summary>
/// This script should contain all Parent transforms that the customization assetsSelected need to be a child of.<br/>
/// </summary>
public class CustomizationAssetsTransformParents : MonoBehaviour
{
    public static CustomizationAssetsTransformParents Instance;

    [SerializedDictionary("Asset Type", "Parent Transform")]
    public SerializedDictionary<CustomizationAssetType, Transform> parentTransformOfAssets;

    [HideInInspector]
    public SerializedDictionary<CustomizationAssetType, GameObject> assetsSelected; // not serialized

    private void Awake()
    {
        // initialize dictionary
        assetsSelected = new();
        foreach (CustomizationAssetType type in Enum.GetValues(typeof(CustomizationAssetType))) {
            assetsSelected.Add(type, default);
        }
    }

    private void OnEnable() => Instance = this; 
    private void OnDestroy() => Instance = null;
}
