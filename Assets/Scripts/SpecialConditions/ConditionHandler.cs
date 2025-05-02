using UnityEngine;

/// <summary>
/// A base class from which special conditions derive from.
/// </summary>
abstract public class ConditionHandler : MonoBehaviour
{
    abstract public string ConditionName { get; }

    abstract public SpecialConditionSO SpecialConditionSO { get; }
}
