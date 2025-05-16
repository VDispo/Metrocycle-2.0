using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is instantiated as a new <see cref="GameObject"/> by <see cref="SpecialConditionsSelector"/> and sits in the mode select screen with it.<br/>
/// This saves all the selected special conditions, and will be alive in the loading of a new scene.
/// </summary>
public class SpecialConditionsSelected : MonoBehaviour
{
    public static SpecialConditionsSelected Instance;

    public Dictionary<string, bool> conditions = new();

    private void Awake()
    {
        if (Instance) Destroy(Instance.gameObject);
        Instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Initialize dictionary
        conditions.Add("Night", false);
        conditions.Add("Rain", false);
        conditions.Add("Fog", false);
    }
}
