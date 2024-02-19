using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Unit : MonoBehaviour
{
    [BoxGroup("Parameters"), SerializeField]
    protected UnitStats unit;
    public UnitStats m_unit { get { return unit; } }
    [BoxGroup("Parameters"), SerializeField]
    protected Transform attackPoint;
    public Transform m_attackPoint { get { return attackPoint; } }
    protected Dictionary<Stats, float> localStats = new Dictionary<Stats, float>();

    private void Awake()
    {
        localStats = new Dictionary<Stats, float>(unit.stats);
    }

    #region Helper Methods
    // NOTE: Use this for debugging projectile spawn point and melee 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, .2f);
    }

    public float GetStat(Stats stat)
    {
        if (localStats.TryGetValue(stat, out float value))
        {
            return value;
        }
        else
        {
            Debug.LogError($"No stat value found for {stat} on {this.name}");
            return 0;
        }
    }

    public float ChangeStat(Stats stat, float amount)
    {
        if (localStats.TryGetValue(stat, out float value))
        {
            localStats[stat] += amount;
            return localStats[stat];
        }
        else
            return -1f;
    }
    #endregion
}
