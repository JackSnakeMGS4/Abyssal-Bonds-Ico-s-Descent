using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Cinemachine;

// TODO: gather all common entity stats for movement, combat, etc. into a UnitStats scriptable object
// and apply the stats to their respective scripts
public class MovementController : MonoBehaviour
{
    [TabGroup("Movement"), SerializeField]
    private float speed, strafingSpeed, dashingSpeed, dashCooldown, knockbackForce, knockbackDuration;
    [SerializeField, Range(0f, 3f), TabGroup("Movement"),
    PropertyTooltip("Modifies range of dash"),
    InfoBox("Subtract from dash cooldown to get desired value I.E. cooldown - limit value")]
    private float dashLimit;
    [TabGroup("Movement"), SerializeField, ReadOnly, ShowInInspector]
    private bool isRecoiling = false;

    [TabGroup("Jumping")]
    [SerializeField] private float jumpHeight, jumpForce, gravityScale;
    [TabGroup("Jumping")]
    [SerializeField] private int numberOfJumps;

    [HorizontalGroup("Split",.3f)]
    [BoxGroup("Split/Dash Debug", centerLabel: true), ShowInInspector, ReadOnly]
    private bool canDash = false;
    [BoxGroup("Split/Dash Debug"), ShowInInspector, ReadOnly]
    private bool isDashing = false;
    public bool m_isDashing { get { return isDashing; } }

    [BoxGroup("Split/Jump Debug", centerLabel: true), ShowInInspector, ReadOnly, SerializeField]
    private float GRAVITY = -9.81f;
    [BoxGroup("Split/Jump Debug"), ShowInInspector, ReadOnly, SerializeField]
    private float tempJumpHeight = 0;
    [BoxGroup("Split/Jump Debug"), ShowInInspector, ReadOnly, SerializeField] 
    private float zPos = 0;
    public float m_zPos { get { return zPos; } }
    [BoxGroup("Split/Jump Debug"), ShowInInspector, ReadOnly, SerializeField]
    private bool isFalling = false;
    public bool m_isFalling { get { return isFalling; } set { isFalling = value; } }
    [BoxGroup("Split/Jump Debug"), ShowInInspector, ReadOnly, SerializeField]
    private bool isJumping = false;
    public bool m_isJumping { get { return isJumping; } }
    [BoxGroup("Split/Jump Debug"), ShowInInspector, ReadOnly, SerializeField]
    private int jumpCount = 0;

    private void OnEnable()
    {
        canDash = true;
        jumpCount = numberOfJumps;
    }

    private void Update()
    {
        ApplyGravity();
    }

    // TODO: make movement have an acceleration and deccel. phase (don't want instant movement to any direction)
    public void Move(Vector2 vector2, bool isStrafing, Rigidbody rb)
    {
        if (isDashing) { return; }
        if (isRecoiling) { return; }

        float horizontal_axis = vector2.x * (isStrafing ? strafingSpeed : speed);
        float vertical_axis = vector2.y * (isStrafing ? strafingSpeed : speed);

        Vector3 new_vel = new Vector3(horizontal_axis, 0, vertical_axis);
        rb.velocity = new_vel;
    }

    /// <summary>
    /// Knock characters in a direction when called. Similar to dash logic but reversed and only 
    /// applies if the character isn't already knocked back to prevent stun lock
    /// </summary>
    /// <param name="recoilDir">Direction that entity will recoil towards</param>
    /// <param name="rb">Rigidbody of recoiling entity</param>
    #region Knockback/Recoil Logic
    public void ApplyRecoil(Vector2 recoilDir, Rigidbody rb)
    {
        if (!isRecoiling) { StartCoroutine(ApplyKnockback(recoilDir, rb)); }
    }

    IEnumerator ApplyKnockback(Vector2 dir, Rigidbody rb)
    {
        isRecoiling = true;
        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isRecoiling = false;
        yield return null;
    }
    #endregion

    /// <summary>
    /// Activates dash when dash button is pressed for a modifiable time limit. Afterwards, it enters a cooldown before it can be used again.
    /// Time limit is used to control distance covered by the dash; meanwhile, cooldown is used to prevent the player from dash-spamming.
    /// </summary>
    /// <param name="vector2"></param>
    /// <param name="rb"></param>
    #region Dash Logic
    public void DoDash(Vector2 vector2, Rigidbody rb)
    {
        if(!canDash) { return; }

        StartCoroutine(Dash(vector2, rb));
    }
    
    IEnumerator Dash(Vector2 vector2, Rigidbody rb)
    {
        isDashing = true;
        canDash = false;
        rb.AddForce(vector2 * dashingSpeed, ForceMode.Impulse);
        yield return new WaitForSeconds(dashLimit);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        yield return null;
    }
    #endregion

    /// <summary> 
    /// Jump to max jump height based and fall down using gravity. 
    /// Detect if jump is initiated in midair and cancel falling until new height is reached.
    /// </summary>
    #region Jumping Logic
    public void Jump()
    {
        if(jumpCount > 0)
        {
            jumpCount--;
            isJumping = true;
            isFalling = false;
            tempJumpHeight = zPos + jumpHeight;
        }
    }

    // TODO: Update for 2.5D use of 3D space
    private void ApplyGravity()
    {
        //Rising
        if (isJumping)
        {
            zPos += jumpForce * Time.deltaTime;
            if(zPos >= tempJumpHeight)
            {
                isJumping = false;
                isFalling = true;
            }
        }
        // Falling
        if (isFalling)
        {
            zPos += GRAVITY * gravityScale * Time.deltaTime;
        }
    }

    public void ResetJump(float floorZ)
    {
        zPos = floorZ;
        isFalling = false;
        jumpCount = numberOfJumps;
    }
    #endregion
}

