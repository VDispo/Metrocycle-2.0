using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script sits in the mode select screen, along with <see cref="SpecialConditionsSelector"/>.<br/>
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
