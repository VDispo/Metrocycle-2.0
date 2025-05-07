using UnityEngine;
using Metrocycle;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System;

/// <summary>
/// This script should contain all Parent transforms that the customization assets need to be a child of.
/// </summary>
public class CustomizationAssetsRefs : MonoBehaviour
{
    public static CustomizationAssetsRefs Instance;

    public BikeType bikeType;

    [SerializedDictionary("Asset Type", "Parent Transform")]
    public SerializedDictionary<CustomizationAssetTypes, Transform> parentOfAssets;
    public Dictionary<CustomizationAssetTypes, GameObject> assets; // not serialized

    private void Awake()
    {
        // initialize dictionary
        assets = new();
        foreach (CustomizationAssetTypes type in Enum.GetValues(typeof(CustomizationAssetTypes))) {
            assets.Add(type, default);
        }
    }

    private void OnEnable() => Instance = this;
    private void OnDestroy() => Instance = null;
}
