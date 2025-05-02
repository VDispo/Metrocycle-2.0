using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script sits inside a specific game mode scene.<br/>
/// It relies on <see cref="SpecialConditionsSelected"/> existing (on DontDestroyOnLoad screen).<br/>
/// Upon getting that object, this script takes its data and activates the respective condition handlers/initiators.
/// </summary>
public class SpecialConditionsInitialHandler : MonoBehaviour
{
    public static SpecialConditionsInitialHandler Instance;

    [Header("Specific Handlers")]
    [SerializeField] private NightConditionHandler nightConditionHandler;
    [SerializeField] private RainConditionHandler rainConditionHandler;
    [SerializeField] private FogConditionHandler fogConditionHandler;
    public Dictionary<ConditionHandler, bool> conditions = new();

    [Header("Vehicle")]
    [SerializeField] private GameObject motorcycle;
    [SerializeField] private GameObject bicycle;
    private bool isMotorcycle;

    [Header("Skybox")]
    [SerializeField] private Skybox[] sideMirrorSkyboxes; // for motorcycle

    [Header("Lighting")]
    [SerializeField] private GameObject defaultLightingPrefab;
    [Tooltip("auto-set to true if any of the condition SO's activateNightLights is true")] 
    public bool activateNightLights;

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        // Initialize each handler
        isMotorcycle = GameManager.Instance.getBikeType() == Metrocycle.BikeType.Motorcycle;
        rainConditionHandler.isMotorcycle = isMotorcycle;
        rainConditionHandler.activeVehicle = isMotorcycle ? motorcycle.transform : bicycle.transform;
        fogConditionHandler.activeVehicle = isMotorcycle ? motorcycle.transform : bicycle.transform;

        // Initialize dictionary containing anyActive states of each condition
        conditions.Add(nightConditionHandler, false);
        conditions.Add(rainConditionHandler, false);
        conditions.Add(fogConditionHandler, false);

        // Copy anyActive states data from conditions (if not found, continue game as normal aka Day Condition)
        SpecialConditionsSelected starter = SpecialConditionsSelected.Instance;
        if (starter)
        {
            ConditionHandler[] keys = new ConditionHandler[conditions.Count]; 
            conditions.Keys.CopyTo(keys, 0);
            foreach (ConditionHandler cond in keys)
            {
                foreach (string condName in starter.conditions.Keys)
                {
                    if (cond.ConditionName == condName)
                        conditions[cond] = starter.conditions[condName];
                }
            }
        }
        else Debug.LogWarning("SpecialConditionsSelected not found. Be sure you're playing from the title screen");
        
        // Actual initialization
        ActivateSpecialConditionHandlers();
        ChangeSkyboxAndLighting();
        ActivateNightLights();
    }

    public void ActivateSpecialConditionHandlers()
    {
        foreach (var entry in conditions)
        {
            entry.Key.gameObject.SetActive(entry.Value);
        }
    }

    private void ChangeSkyboxAndLighting()
    {
        ConditionHandler[] keys = conditions.Keys.ToArray();
        bool[] values = conditions.Values.ToArray();

        Material chosenSkyboxMat = null;
        GameObject chosenLightPrefab = defaultLightingPrefab;
        int skyboxHighestPriority = int.MinValue;
        int lightingHighestPriority = int.MinValue;
        for (int i = 0; i < values.Length; i++)
        {
            if (!values[i]) continue; // skip those not selected

            // get the highest priority skybox
            int _skyboxPrio = keys[i].SpecialConditionSO.skyboxPriority;
            if (_skyboxPrio > skyboxHighestPriority)
            {
                skyboxHighestPriority = _skyboxPrio;
                chosenSkyboxMat = keys[i].SpecialConditionSO.skybox;
            }

            // get the highest priority lighting
            int _lightingPrio = keys[i].SpecialConditionSO.lightingPriority;
            if (_lightingPrio > lightingHighestPriority)
            {
                lightingHighestPriority = _lightingPrio;
                chosenLightPrefab = keys[i].SpecialConditionSO.lightingPrefab;
            }
        }

        if (chosenSkyboxMat)
        {
            Camera.main.GetComponent<Skybox>().material = chosenSkyboxMat;
            if (isMotorcycle)
            {
                foreach (Skybox comp in sideMirrorSkyboxes)
                    comp.material = chosenSkyboxMat;
            }
            Debug.Log($"[{GetType().FullName}] mode initialized with the skybox {chosenSkyboxMat}");
        }

        Instantiate(chosenLightPrefab);
        Debug.Log($"[{GetType().FullName}] mode initialized with the lighting {chosenLightPrefab}");
    }

    /// <summary>
    /// Note that this must be called BEFORE <see cref="ActivateSpecialConditionHandlers"/>, since it is only then that the condition handlers are activated.
    /// </summary>
    private void ActivateNightLights()
    {
        bool anyActive =
            (fogConditionHandler.isActiveAndEnabled && fogConditionHandler.SpecialConditionSO.activateNightLights) ||
            (nightConditionHandler.isActiveAndEnabled && nightConditionHandler.SpecialConditionSO.activateNightLights) ||
            (rainConditionHandler.isActiveAndEnabled && rainConditionHandler.SpecialConditionSO.activateNightLights);
        activateNightLights = anyActive;
    }
}