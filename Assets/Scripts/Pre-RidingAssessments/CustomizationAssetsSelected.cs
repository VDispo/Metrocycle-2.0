using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// ALMOST DEPRECATED -- everything related to the idea of keeping assets in play time !
/// <summary>
/// This script survives scene change and is responsible for holding the player's selected assets.
/// </summary>
public class CustomizationAssetsSelected : MonoBehaviour
{
    public static CustomizationAssetsSelected Instance;

    [Header("Refs")]
    public Dictionary<CustomizationAssetSetSO, int> assetSetsSelection;
    public Dictionary<CustomizationAssetSetSO, bool> gearValidity;

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    /// <summary>
    /// Called by <see cref="CharacterCustomizationHandler"/>
    /// </summary>
    public void Initialize()
    {
        assetSetsSelection = new();
        foreach (CustomizationAssetSetSO so in CharacterCustomizationHandler.Instance.assetSets)
            assetSetsSelection.Add(so, 0); // default selection is 0

        gearValidity = new();
        foreach (CustomizationAssetSetSO assetSet in CharacterCustomizationHandler.Instance.assetSets)
            SaveAssetSelection(assetSet, 0);
    }

    // TODO clean
    public void SaveAssetSelection(CustomizationAssetSetSO so, int idx)
    {
        if (so && idx >= 0 && idx < so.choicesPrefabsWithPassing.Count)
            assetSetsSelection[so] = idx;
        else Debug.LogError($"[{GetType().FullName}] cannot save asset with so {so} and idx {idx}");

        GameObject chosenGO = so.choicesPrefabsWithPassing.Keys.ToArray()[idx];
        if (gearValidity.ContainsKey(so))
            gearValidity[so] = so.choicesPrefabsWithPassing[chosenGO];
        else gearValidity.Add(so, so.choicesPrefabsWithPassing[chosenGO]);
        
        Debug.Log($"[{GetType().FullName}] chosen gear: {chosenGO}, validity: {gearValidity}");
    }

    public bool AllGearsValid()
    {
        foreach(bool b in gearValidity.Values)
        {
            Debug.Log($"[{GetType().FullName}] b: {b}");
            if (b == false) return false;
        }
        return true;
    }
}
