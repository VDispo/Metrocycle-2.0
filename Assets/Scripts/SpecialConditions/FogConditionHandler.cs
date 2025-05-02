using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FogConditionHandler : ConditionHandler
{
    public override string ConditionName { get => "Fog"; }
    public override SpecialConditionSO SpecialConditionSO { get => _associatedSpecialConditionSO; }

    [Header("Associated Scriptable Object")]
    [SerializeField] private SpecialConditionSO _associatedSpecialConditionSO;

    //[Header("Vehicle")]
    [HideInInspector] public bool isMotorcycle = false;
    [HideInInspector] public Transform activeVehicle;

    [Header("Fog Shader")]
    [SerializeField][Tooltip("One for each graphics mode (Performant, Balanced, High Fidelity)")] private UniversalRendererData[] dataRendererURP;
    private ScriptableRendererFeature[] cachedFeatures;

    private void Start()
    {
        cachedFeatures = new ScriptableRendererFeature[dataRendererURP.Length];

        int idx = 0;
        foreach (UniversalRendererData rend in dataRendererURP) {
            foreach (ScriptableRendererFeature feat in rend.rendererFeatures)
            {
                if (feat.GetType() == typeof(FullScreenPassRendererFeature))
                {
                    cachedFeatures[idx++] = feat;
                    break;
                }
            }
        }

        ToggleFog(true);
    }

    /// <summary>
    /// Sets the active state of all cached <see cref="FullScreenPassRendererFeature"/> to the <paramref name="targetState"/> (toggles the bool state if none provided).
    /// </summary>
    /// <param name="targetState"></param>
    private void ToggleFog(bool? targetState = null)
    {
        foreach (ScriptableRendererFeature feat in cachedFeatures)
        {
            if (feat) feat.SetActive(targetState ?? !feat.isActive);
        }
    }

    private void OnDestroy()
    {
        // Ensures fog renderer (which is an asset) returns to its normal (inactive) state after scene changes or closing the game.
        ToggleFog(false);
    }
}
