using AYellowpaper.SerializedCollections;
using UnityEngine;
using Metrocycle;
using System.Linq;

/// <summary>
/// To add an asset, <br/>
/// 1. Create a prefab variant containing the asset <br/>
/// 2. Assign the prefab variant into a new or existing CustomizationAssetSetSO (into the choicesPrefabs array) <br/>
/// -- If new, <br/>
/// ---- add a new entry in the CustomizationAssetType enum in Constants.cs and assign it to the new CustomizationAssetSetSO <br/>
/// ---- in the Pre-riding_Assessment scene, find the Bike prefab object and modify each of the childrens' CustomizationAssetsTransformParents script
///             to include a new dictionary entry (key: the new CustomizationAssetSetSO created, progress: the parent transform of the Bi/Motorcycle to instatiate under)<br/>
/// ---- in the same scene, find the Canvas > CharacterCustomization object and add the new scriptable object into the AssetSets array
/// </summary>
public class CharacterCustomizationHandler : MonoBehaviour
{
    public static CharacterCustomizationHandler Instance;

    [Header("Refs")]
    [SerializeField]
    [SerializedDictionary("Vehicle", "Customization Sets")]
    private SerializedDictionary<BikeType, CustomizationAssetSetSO[]> assetSets;

    [Space(10)]
    [SerializeField] private Transform selectorsParent;
    [SerializeField] private GameObject selectorPrefab;
    [HideInInspector] public CustomizationAssetSetSelector[] activeSelectors;


    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        // Initialize all selector UI
        activeSelectors = new CustomizationAssetSetSelector[assetSets[default].Length]; // 1 selector per customization set

        int idx = 0;
        foreach (CustomizationAssetSetSO set in assetSets[PreRidingAssessmentUiHandler.vehicleType])
        {
            CustomizationAssetSetSelector newUiInstance = Instantiate(selectorPrefab, selectorsParent).GetComponent<CustomizationAssetSetSelector>();
            CustomizationAssetSetSO cachedSet = set;

            newUiInstance.Initialize(cachedSet);
            activeSelectors[idx++] = newUiInstance;
        }
    }

    /// <summary> Returns true if no invalid/failing.</summary>
    public bool AllGearsValid()
        => !activeSelectors.Any((CustomizationAssetSetSelector selector) => selector.selectedValid == false);
}
