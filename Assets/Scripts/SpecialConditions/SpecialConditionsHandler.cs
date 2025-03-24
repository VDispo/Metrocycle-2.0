using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialConditionsHandler : MonoBehaviour
{
    [SerializeField] private RainConditionHandler rainConditionHandler;
    //[SerializeField] private ConditionHandler nightConditionHandler;
    //[SerializeField] private ConditionHandler fogConditionHandler;
    public Dictionary<ConditionHandler, bool> specialConditionsInvolved = new();

    private void Start()
    {
        // Initialize dictionary
        specialConditionsInvolved.Add(rainConditionHandler, false);
        //specialConditionsInvolved.Add(nightConditionHandler, false);
        //specialConditionsInvolved.Add(fogConditionHandler, false);

        foreach (var entry in specialConditionsInvolved)
        {
            entry.Key.enabled = entry.Value;
        }

        specialConditionsInvolved[rainConditionHandler] = true; // debug 
        InitializeConditions(); // debug

        //// This is how it will be set in the Select Mode scene:
        // specialConditionsInvolved(or .values) = <dictionary or ordered array input from select mode>
    }

    public List<string> GetConditionNames()
    {
        List<string> conditionNamesOrdered = new(specialConditionsInvolved.Count);

        foreach(ConditionHandler handler in specialConditionsInvolved.Keys)
        {
            conditionNamesOrdered.Add(handler.ConditionName);
        }

        return conditionNamesOrdered;
    }

    public void InitializeConditions()
    {
        foreach (var entry in specialConditionsInvolved)
        {
            entry.Key.enabled = entry.Value;
        }
    }
}