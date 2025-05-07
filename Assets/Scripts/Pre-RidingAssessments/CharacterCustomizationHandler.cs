using UnityEngine;

public class CharacterCustomizationHandler : MonoBehaviour
{
    public static CharacterCustomizationHandler Instance;

    public CustomizationAssetSetSO[] assetSets;
    [SerializeField] private GameObject uiPrefab;

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        foreach(CustomizationAssetSetSO set in assetSets)
        {
            CustomizationAssetSetSelector newUiInstance = Instantiate(uiPrefab, transform).GetComponent<CustomizationAssetSetSelector>();
            CustomizationAssetSetSO cachedSet = set;
            newUiInstance.Initialize(cachedSet);
        }
    }
}
