using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightConditionHandler : ConditionHandler
{
    public override string ConditionName { get => "Night"; }
    public override SpecialConditionSO SpecialConditionSO { get => _specialConditionSO; }
    
    [Header("Associated Scriptable Object")]
    [SerializeField] private SpecialConditionSO _specialConditionSO;

    /// TODO
    /// - replace sun
    /// - activate nightLight sources (maybe via tag or layer or custimized script)
    /// - maybe add thicc vignette
}
