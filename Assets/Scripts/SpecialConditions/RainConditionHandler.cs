using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainConditionHandler : ConditionHandler
{
    public override string ConditionName { get => "Rain"; }
    public override SpecialConditionSO SpecialConditionSO { get => _associatedSpecialConditionSO; }
    
    [Header("Associated Scriptable Object")]
    [SerializeField] private SpecialConditionSO _associatedSpecialConditionSO;

    //[Header("Vehicle")]
    [HideInInspector] public bool isMotorcycle = false;
    [HideInInspector] public Transform activeVehicle;

    [Header("Rain Particles")]
    [SerializeField] private Transform rainParticlesTransform;
    [SerializeField][Tooltip("interval [in seconds] of updating rain particle position to follow player vehicle")] private float particlesPositionUpdateInterval = 1; 
    private float startOffsetZ = 0; // Z of particle's center is offset forward since the player sees more things in the forward direction (its better to put more particles there than at the back)

    private void Start()
    {
        startOffsetZ = rainParticlesTransform.localPosition.z;
        StartCoroutine(nameof(UpdateParticlePosition));
    }
    
    private IEnumerator UpdateParticlePosition()
    {
        rainParticlesTransform.localPosition = new Vector3(
            activeVehicle.localPosition.x,
            rainParticlesTransform.localPosition.y,
            activeVehicle.localPosition.z + startOffsetZ);
        yield return new WaitForSecondsRealtime(particlesPositionUpdateInterval);
        StartCoroutine(nameof(UpdateParticlePosition));
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
