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

        SpecialConditionsInitializer starter = FindFirstObjectByType<SpecialConditionsInitializer>();
        ConditionHandler[] keys = new ConditionHandler[specialConditionsInvolved.Count]; 
        specialConditionsInvolved.Keys.CopyTo(keys, 0); // unsure if all will be copied
        foreach (ConditionHandler cond in keys)
        {
            foreach (string condName in starter.specialConditionsInvolved.Keys)
            {
                if (cond.ConditionName == condName)
                    specialConditionsInvolved[cond] = starter.specialConditionsInvolved[condName];
            }
        }

        InitializeConditions();
        Destroy(starter.gameObject);
    }

    //public List<string> GetConditionNames()
    //{
    //    List<string> conditionNamesOrdered = new(specialConditionsInvolved.Count);

    //    foreach(ConditionHandler handler in specialConditionsInvolved.Keys)
    //    {
    //        conditionNamesOrdered.Add(handler.ConditionName);
    //    }

    //    return conditionNamesOrdered;
    //}

    public void InitializeConditions()
    {
        foreach (var entry in specialConditionsInvolved)
        {
            entry.Key.gameObject.SetActive(entry.Value);
        }
    }
}