using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialConditionsInitializer : MonoBehaviour
{
    public Dictionary<string, bool> specialConditionsInvolved = new();

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Initialize dictionary
        specialConditionsInvolved.Add("Rain", false);
        //specialConditionsInvolved.Add("Night", false);
        //specialConditionsInvolved.Add("Fog", false);
    }
}
