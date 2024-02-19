using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MovementController movementController;
    private InputController inputController;
    // private AnimationController animController;
    private CollisionsController collisionsController;
    private Ranged rangedController;
    // private Melee meleeController;

    void Awake()
    {
        inputController = GetComponent<InputController>();
        movementController = GetComponent<MovementController>();
        // animController = GetComponent<AnimationController>();
        collisionsController = GetComponent<CollisionsController>();
        rangedController = GetComponent<Ranged>();
        // meleeController = GetComponent<Melee>();
    }

    // TODO: Remember to unsubscribe all events when game object is no longer active
    private void Start()
    {
        inputController.AssignGameplayControls();
        // animController.SetupSprite();
        collisionsController.SetupCollisions();
        SetupShootingInputEvents();
        // SetupMeleeInputEvents();

        inputController.inputActions.Player.Dash.performed += _ =>
        movementController.DoDash(inputController.MovementInput(), collisionsController.m_rb);

    }

    private void Update()
    {
        // UpdateCurrentTileLayer();
        // DetectGroundCollision();
        // UpdateMeleeDirection();
        // UpdateAnimController();
        UpdateAim();
    }

    private void FixedUpdate()
    {
        movementController.Move(inputController.MovementInput(), inputController.IsAiming(), collisionsController.m_rb);
    }

    #region Helper Methods
    // private void UpdateMeleeDirection()
    // {
    //     if (inputController.IsMoving())
    //     {
    //         meleeController.m_lastKnownDir = inputController.MovementInput();
    //     }
    // }

    // private void SetupMeleeInputEvents()
    // {
    //     inputController.inputActions.Player.Melee.performed += _ =>
    //     animController.PlayBasicMeleeAnim(meleeController.m_canBasicMelee);
    //     inputController.inputActions.Player.Melee.performed += _ =>
    //     meleeController.BasicHit();

    //     #region Heavy Melee Input Setup
    //     inputController.inputActions.Player.Melee2.performed += _ =>
    //     animController.PlayHeavyMeleeAnim(meleeController.m_canHeavyMelee);
    //     inputController.inputActions.Player.Melee2.performed += _ =>
    //     meleeController.ChargedHit();
    //     inputController.inputActions.Player.Melee2.performed += _ =>
    //     CameraShakeController.Instance.ShakeOnHeavyHit();
    //     #endregion
    // }

    private void SetupShootingInputEvents()
    {
        inputController.inputActions.Player.Fire.performed += _ =>
        rangedController.Fire(inputController.m_targetPos, inputController.IsAiming());
        inputController.inputActions.Player.Fire.performed += _ =>
        //animController.PlayBasicShotAnim(inputController.IsAiming());

        #region Heavy Shot Input Setup
        inputController.inputActions.Player.Fire2.performed += _ =>
        rangedController.FireCharged(inputController.m_targetPos, inputController.IsAiming());
        inputController.inputActions.Player.Fire2.performed += _ =>
        //animController.PlayHeavyShotAnim(inputController.IsAiming());
        inputController.inputActions.Player.Fire2.performed += _ =>
        movementController.ApplyRecoil(rangedController.m_recoilDir, collisionsController.m_rb);
        /*inputController.inputActions.Player.Fire2.performed += _ =>
        CameraShakeController.Instance.ShakeOnRecoil(collisionsController.m_rb.position, rangedController.m_recoilDir);*/
        #endregion
    }

    // private void DetectGroundCollision()
    // {
    //     if (collisionsController.CollidedWithGround(movementController.m_zPos))
    //     {
    //         animController.PlayLandingAnim();
    //         movementController.ResetJump(collisionsController.m_floorZ);
    //     }
    // }

    private bool IsFacingLeft()
    {
        return inputController.MovementInput().x < 0;
    }

    // private void UpdateAnimController()
    // {
    //     if(inputController.IsMoving())
    //     {
    //         animController.FlipSprite(IsFacingLeft());
    //         animController.PlayDashAnim(movementController.m_isDashing);
    //     }
    //     animController.PlayMovementAnim(inputController.IsMoving());
    //     animController.PlayJumpAnim(movementController.m_isJumping);
    //     //set sprite y to z pos from movement controller
    //     animController.ApplyJumpToSprite(movementController.m_zPos - collisionsController.m_floorZ);
    // }

    private void UpdateAim()
    {
        if (inputController.IsAiming())
        {
            inputController.Aim(rangedController.m_attackPoint);
            // animController.DrawAimingInterface(inputController.m_targetPos, rangedController.m_attackPoint.position, inputController.IsAiming());
        }
        else
        {
            inputController.ResetAim(rangedController.m_attackPoint);
            // animController.ResetAimingInterface(rangedController.m_attackPoint.position, inputController.IsAiming());
        }
    }
    #endregion
}
