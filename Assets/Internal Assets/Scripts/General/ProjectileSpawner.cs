using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Sirenix.OdinInspector;

public class ProjectileSpawner : MonoBehaviour
{
    // TODO: fill pool with projectiles and allow entities that implement Ranged class to use projectile from it
    [BoxGroup("Projectile Pool"), SerializeField]
    private Projectile projectilePrefab;
    [BoxGroup("Projectile Pool"), SerializeField]
    private int maxSize;
    [BoxGroup("Projectile Pool"), ReadOnly, ShowInInspector, SerializeField]
    private ObjectPool<Projectile> projectilePool;
    public ObjectPool<Projectile> m_projectilePool { get { return projectilePool; } }
    [BoxGroup("Projectile Pool"), ReadOnly, ShowInInspector, SerializeField]
    private int inactiveCount;
    [BoxGroup("Projectile Pool"), ReadOnly, ShowInInspector, SerializeField]
    private int activeCount;

    private void Awake()
    {
        // NOTE: collectionCheck is false since pool only checks for Editor sessions
        // when it's true it throws an error; however, builds ignore this setting so it's best to have
        // set to false during development
        projectilePool = new ObjectPool<Projectile>(CreateProjectile, actionOnGet: OnTakeProjectile, actionOnRelease: OnReturnProjectile, collectionCheck: false, maxSize: maxSize);
    }

    private void Update()
    {
        inactiveCount = projectilePool.CountInactive;
        activeCount = projectilePool.CountActive;
    }

    // Is called whenever pool is empty to create new projectiles
    private Projectile CreateProjectile()
    {
        var projectile = Instantiate(projectilePrefab);
        projectile.SetPool(projectilePool);
        return projectile;
    }

    private void OnTakeProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    private void OnReturnProjectile(Projectile projectile)
    {
        // get hit effects/particles from particle/vfx pools and set their pos to projectile pos after impact
        projectile.gameObject.SetActive(false);
    }
}
