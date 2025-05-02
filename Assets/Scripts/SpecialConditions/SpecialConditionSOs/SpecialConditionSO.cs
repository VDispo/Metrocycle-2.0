using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialConditionSO", menuName = "SpecialConditionSO")]
public class SpecialConditionSO : ScriptableObject
{
    [Header("Skybox")]
    public Material skybox;
    [Tooltip("higher means more priority")] public int skyboxPriority;
    
    [Header("Lighting")]
    public GameObject lightingPrefab;
    [Tooltip("higher means more priority")] public int lightingPriority;
    public bool activateNightLights; // such as headlights and street lamps
}
