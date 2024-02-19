using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

public class Projectile : MonoBehaviour, IProjectile
{
    [BoxGroup("Projectile Settings"), SerializeField, PropertyRange(0,30)]
    private float force;
    [BoxGroup("Projectile Settings"), SerializeField, ListDrawerSettings(NumberOfItemsPerPage = 3)]
    private List<Sprite> usableSprites;
    [BoxGroup("Projectile Settings"), SerializeField, ListDrawerSettings(NumberOfItemsPerPage = 3)]
    private List<RuntimeAnimatorController> usableAnimControllers;

    [BoxGroup("Debug"), ReadOnly, ShowInInspector]
    private IObjectPool<Projectile> thisProjectilePool;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private Rigidbody2D rb;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private SortingGroup sortGroup;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private GameObject spawningEntity;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private float maxRange;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private float damage;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sortGroup = GetComponent<SortingGroup>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    public void SetPool(IObjectPool<Projectile> pool)
    {
        thisProjectilePool = pool;
    }

    public void ApplyForce(Vector2 dir)
    {
        rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

    private void Update()
    {
        RemoveOutOfRangeProjectiles();
    }

    // TODO: reset projectile state after it's returned to the pool (to prevent any potential data bleed through)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var otherProjectile = collision.gameObject.GetComponent<IProjectile>();
        var damageable = collision.gameObject.GetComponent<IDamageable>();

        if (otherProjectile != null) { CleanUpProjectile(); return; }
        if (collision.gameObject == spawningEntity) { CleanUpProjectile(); return; }
        if (damageable == null) { CleanUpProjectile(); return; }

        damageable.TakeDamage(damage);
        CleanUpProjectile();
    }

    #region Helper Methods
    private void CleanUpProjectile()
    {
        if (thisProjectilePool != null)
        {
            thisProjectilePool.Release(element: this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLayersAndRange(GameObject spawningEntity, float maxRange)
    {
        this.maxRange = maxRange;

        this.spawningEntity = spawningEntity;
        gameObject.layer = spawningEntity.layer;
        sortGroup.sortingLayerName = LayerMask.LayerToName(spawningEntity.layer);
        sortGroup.sortingOrder = spawningEntity.GetComponent<SortingGroup>().sortingOrder;

        // Ignore collisions betweens this projectile and its parent
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), this.spawningEntity.GetComponent<Collider2D>());
    }

    public void SetSpriteAndAnimator(bool isCharged)
    {
        if (isCharged)
        {
            sprite.sprite = usableSprites[1];
            animator.runtimeAnimatorController = usableAnimControllers[1];
        }
        else
        {
            sprite.sprite = usableSprites[0];
            animator.runtimeAnimatorController = usableAnimControllers[0];
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void RemoveOutOfRangeProjectiles()
    {
        if (Vector2.Distance(gameObject.transform.position, spawningEntity.transform.position) >= maxRange)
        {
            CleanUpProjectile();
        }
    }
    #endregion
}
