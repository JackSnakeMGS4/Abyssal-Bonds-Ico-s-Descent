using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface IProjectile
{
    void ApplyForce(Vector3 dir);
    void SetPool(IObjectPool<Projectile> pool);
    void SetLayersAndRange(GameObject spawningEntity, float maxRange);
}
