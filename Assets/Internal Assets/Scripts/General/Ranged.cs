using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Pool;

public class Ranged : Unit, IShoot
{
    [BoxGroup("Projectile Pool"), SerializeField]
    private ProjectileSpawner projectileSpawner;
    [BoxGroup("Heat Dissipation Timer"), SerializeField, PropertyRange(0.1f, 3f)]
    private float reductionTimer = 2f;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private bool canFire = true;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private bool isOverheated = false;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField, 
        ProgressBar(0, 100, ColorGetter = "GetBarColor", Height = 20)]
    private float currentHeatBuildup = 0f;
    public float m_heatBuildup { get { return currentHeatBuildup; } }
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private Vector2 recoilDir;
    public Vector2 m_recoilDir { get { return recoilDir; } }

    private void OnEnable()
    {
        InvokeRepeating("ReduceThermalBuildup", 1f, reductionTimer);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void ReduceThermalBuildup()
    {
        if (isOverheated) { return; }
        if (currentHeatBuildup < 0) { currentHeatBuildup = 0f; return; }

        currentHeatBuildup -= GetStat(Stats.thermalReductionPercentage);
    }

    #region Shooting Logic
    public void Fire(Vector3 targetPos, bool isAiming)
    {
        if (!canFire) return;
        if (isOverheated) return;
        if (!isAiming) return;
        
        StartCoroutine(ShootIfAble(targetPos, false));
    }

    public void FireCharged(Vector3 targetPos, bool isAiming)
    {
        if (!canFire) return;
        if (isOverheated) return;
        if (!isAiming) return;

        StartCoroutine(ShootIfAble(targetPos, true));
    }

    /// <summary>
    /// Shoots projectile (type based on input) towards the aiming direction (based on target position)
    /// and increases thermal buildup for overheating feature
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="isCharged"></param>
    /// <returns></returns>
    IEnumerator ShootIfAble(Vector3 vector, bool isCharged)
    {
        canFire = false;
        var projectile = projectileSpawner.m_projectilePool.Get();
        if (projectile != null)
        {
            projectile.transform.position = attackPoint.position;
            // set its rotation and orientiation
            projectile.transform.rotation = HelperMethods.GetRotationToDir(HelperMethods.GetDirFromOrigin(vector, attackPoint.position));

            projectile.SetLayersAndRange(this.gameObject, GetStat(Stats.firearmRange));
            projectile.SetSpriteAndAnimator(isCharged);

            if(isCharged)
            {
                recoilDir = -HelperMethods.GetDirFromOriginNormalized(vector, attackPoint.position);
                projectile.SetDamage(GetStat(Stats.rangedDamage) * GetStat(Stats.chargedRangeDmgMod));
                currentHeatBuildup += GetStat(Stats.chargedThermalBuildup);
            }
            else
            {
                projectile.SetDamage(GetStat(Stats.rangedDamage));
                currentHeatBuildup += GetStat(Stats.standardThermalBuildup);
            }

            projectile.ApplyForce(HelperMethods.GetDirFromOriginNormalized(vector, attackPoint.position));
            if(currentHeatBuildup >= GetStat(Stats.thermalCapacity))
            {
                isOverheated = true;
                StartCoroutine(OverheatCooldown());
            }
        }
        yield return new WaitForSeconds(GetStat(Stats.fireRate));
        canFire = true;
        yield return null;
    }
    #endregion

    IEnumerator OverheatCooldown()
    {
        yield return new WaitForSeconds(GetStat(Stats.overheatCooldownTime));
        isOverheated = false;
        currentHeatBuildup = 0f;
    }

    #region Helper Methods
    private Color GetBarColor(float value)
    {
        return Color.Lerp(Color.blue, Color.red, Mathf.Pow(value / 1f, 2));
    }
    #endregion
}
