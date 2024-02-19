using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum Stats
{
    health,
    barriers,
    rangedDamage,
    firearmRange,
    fireRate,
    rangedChargeTime,
    chargedRangeDmgMod,
    thermalCapacity,
    standardThermalBuildup,
    chargedThermalBuildup,
    thermalReductionPercentage,
    overheatCooldownTime,
    meleeDamage,
    shortMeleeRange,
    longMeleeRange,
    meleeRate,
    meleeChargeTime,
    chargedMeleeDmgMod,
    chargedMeleeCooldown
}

[CreateAssetMenu(fileName = "Stats_", menuName = "Unit Stats", order = 1)]
public class UnitStats : SerializedScriptableObject
{
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine)]
    public Dictionary<Stats, float> stats = new Dictionary<Stats, float>();
}
