using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script survives scene change and is responsible for holding the player's selected assets.
/// </summary>
public class CustomizationAssetsSelected : MonoBehaviour
{
    public static CustomizationAssetsSelected Instance;

    [Header("Refs")]
    private Dictionary<CustomizationAssetSetSO, int> assetSetsSelection;

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        assetSetsSelection = new();
        foreach(CustomizationAssetSetSO so in CharacterCustomizationHandler.Instance.assetSets)
        {
            assetSetsSelection.Add(so, default); // default selection is 0
        }
    }

    public void SaveAssetSelection(CustomizationAssetSetSO so, int idx)
    {
        if (so && idx >= 0 && idx < so.choicesPrefabs.Length)
            assetSetsSelection[so] = idx;
        else Debug.LogError($"[{GetType().FullName}] cannot save asset with so {so} and idx {idx}");
    }
}
