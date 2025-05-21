using UnityEngine;

/// <summary>
/// To add an asset, <br/>
/// 1. Create a prefab variant containing the asset <br/>
/// 2. Assign the prefab variant into a new or existing CustomizationAssetSetSO (into the choicesPrefabs array) <br/>
/// -- If new, <br/>
/// ---- add a new entry in the CustomizationAssetType enum in Constants.cs and assign it to the new CustomizationAssetSetSO <br/>
/// ---- in the Pre-riding_Assessment scene, find the Bike prefab object and modify each of the childrens' CustomizationAssetsRefs script
///             to include a new dictionary entry (key: the new CustomizationAssetSetSO created, progress: the parent transform of the Bi/Motorcycle to instatiate under)<br/>
/// ---- in the same scene, find the Canvas > CharacterCustomization object and add the new scriptable object into the AssetSets array
/// </summary>
public class CharacterCustomizationHandler : MonoBehaviour
{
    public static CharacterCustomizationHandler Instance;

    public CustomizationAssetSetSO[] assetSets;
    [SerializeField] private GameObject uiPrefab;

    [HideInInspector] public CustomizationAssetSetSelector[] selectors;

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        selectors = new CustomizationAssetSetSelector[assetSets.Length];

        CustomizationAssetsSelected.Instance.Initialize();

        int idx = 0;
        foreach (CustomizationAssetSetSO set in assetSets)
        {
            CustomizationAssetSetSelector newUiInstance = Instantiate(uiPrefab, transform).GetComponent<CustomizationAssetSetSelector>();
            CustomizationAssetSetSO cachedSet = set;

            newUiInstance.Initialize(cachedSet);
            selectors[idx++] = newUiInstance;
            newUiInstance.gameObject.SetActive(false);
        }
    }
}
