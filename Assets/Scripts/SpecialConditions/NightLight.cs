using System.Linq;
using UnityEngine;

/// <summary>
/// This should be appended to game objects that have a <see cref="Light"/> component that is off by default, and should be turned on if the lighting conditions deem it (i.e., it's a night light).<br/>
/// Note that this script runs <c>15ms</c> delayed from default script start time to allow <see cref="SpecialConditionsInitialHandler"/> to finish its logic.
/// </summary>
public class NightLight : MonoBehaviour
{
    [SerializeField] private Light nightLight;
    
    private void Start()
    {
        nightLight.enabled = SpecialConditionsInitialHandler.Instance.activateNightLights;
    }
}
