using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

// TODO: Refactor into parent class and inherit from it to separate player and AI logic
public class GraphicsController : MonoBehaviour
{
    [BoxGroup("Reticle"), SerializeField]
    private GameObject reticle;
    public GameObject m_reticle { get { return reticle; } }

    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private Animator animator;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private GameObject spriteGameObject;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private SpriteRenderer sprite;
    public SpriteRenderer m_sprite { get { return sprite; } }
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private Vector3 originalTransform;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private SortingGroup sortGroup;
    public SortingGroup m_sortGroup { get { return sortGroup; } }
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private LineRenderer aimLine;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private SpriteRenderer crosshair;

    /// <summary>
    /// Draws player aiming reticle and aiming line/direction
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="attackPoint"></param>
    /// <param name="isAiming"></param>
    public void DrawAimingInterface(Vector2 targetPos, Vector2 attackPoint, bool isAiming)
    {
        aimLine.SetPosition(0, attackPoint);
        aimLine.SetPosition(1, targetPos);
        reticle.transform.position = targetPos;
        reticle.SetActive(isAiming);
    }

    public void ResetAimingInterface(Vector2 attackPoint, bool isAiming)
    {
        reticle.transform.position = attackPoint;
        reticle.SetActive(isAiming);
    }

    /// <summary>
    /// Sets the gameobjects containing a sprite renderers and sets its original trasforms
    /// </summary>
    /// <param name="sprite"></param>
    public void SetupSprite()
    {
        sortGroup = GetComponent<SortingGroup>();
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        spriteGameObject = sprite.gameObject ;
        originalTransform = spriteGameObject.transform.localPosition;

        aimLine = reticle.GetComponent<LineRenderer>();
        crosshair = reticle.GetComponent<SpriteRenderer>();
    }

    public void FlipSprite(bool shouldFlip)
    {
        sprite.flipX = shouldFlip;
    }

    public void ApplyJumpToSprite(float zPos)
    {
        // TODO: fix sprites shifting from parent transform due to different pivot points on sprite sheets
        spriteGameObject.transform.localPosition = new Vector2(originalTransform.x, originalTransform.y + zPos);
    }

    public void PlayMovementAnim(bool isMoving)
    {
        animator.SetBool("Is Moving", isMoving);
    }

    public void PlayJumpAnim(bool isJumping)
    {
        animator.SetBool("Is Jumping", isJumping);
    }

    public void PlayLandingAnim()
    {
        animator.Play("Landing");
    }

    public void PlayDashAnim(bool isDashing)
    {
        animator.SetBool("Is Dashing", isDashing);
    }

    public void PlayBasicShotAnim(bool isAiming)
    {
        if(isAiming)
            animator.Play("Basic Shot");
    }

    public void PlayHeavyShotAnim(bool isAiming)
    {
        if(isAiming)
            animator.Play("Heavy Shot");
    }
    
    public void PlayBasicMeleeAnim(bool canMelee)
    {
        if(canMelee)
            animator.Play("Basic Melee");
    }

    public void PlayHeavyMeleeAnim(bool canMelee)
    {
        if(canMelee)
            animator.Play("Heavy Melee");
    }
}
