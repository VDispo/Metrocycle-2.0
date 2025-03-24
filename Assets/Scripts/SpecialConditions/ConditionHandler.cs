using UnityEngine;

abstract public class ConditionHandler : MonoBehaviour
{
    // Just a base class from which special conditions derive from
    abstract public string ConditionName { get; }
}
