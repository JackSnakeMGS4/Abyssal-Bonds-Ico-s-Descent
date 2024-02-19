using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class InputController : MonoBehaviour
{
    [BoxGroup("Camera Peek"), SerializeField, 
    InfoBox("Used to determine how far the player's aiming should peek ahead"),
    PropertyRange(0, 10)]
    private float maxPeekDistance;

    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private Vector3 targetPos;
    public Vector3 m_targetPos { get { return targetPos; } set { targetPos = value; } }
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private bool isGamepad = false;
    public bool m_isGamepad { get { return isGamepad; } }
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private bool aimInputRegistering;
    [BoxGroup("Debug"), ReadOnly, ShowInInspector, SerializeField]
    private Vector2 moveInputVector;

    public PlayerInput playerInput { get; private set; }
    public PlayerControls inputActions { get; private set; }
    public InputAction Movement { get; private set; }
    public InputAction Aiming { get; private set; }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inputActions = new PlayerControls();
        
        targetPos = new Vector3();
    }

    private void Update()
    {
        aimInputRegistering = IsAiming();
        moveInputVector = MovementInput();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    public void AssignGameplayControls()
    {
        Movement = inputActions.Player.Move;
        Aiming = inputActions.Player.Aim;

        playerInput.onControlsChanged += _ => IsGamepad();
    }

    public void Aim(Transform attackPoint)
    {
        if (isGamepad)
        {
            targetPos = new Vector3(attackPoint.position.x + Aiming.ReadValue<Vector2>().x * maxPeekDistance, 0,
                attackPoint.position.y + Aiming.ReadValue<Vector2>().y * maxPeekDistance);
        }
        else
        {
            Vector2 pos = playerInput.camera.ScreenToWorldPoint(Aiming.ReadValue<Vector2>());
            Vector2 dir = pos - (Vector2) attackPoint.position;
            dir.Normalize();
            targetPos = new Vector3(attackPoint.position.x + dir.x * maxPeekDistance, 0, attackPoint.position.y + dir.y * maxPeekDistance);
        }
    }

    public void ResetAim(Transform attackPoint)
    {
        targetPos = attackPoint.transform.position;
    }

    #region Helper Methods
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPos, .2f);
    }

    public Vector2 MovementInput()
    {
        return Movement.ReadValue<Vector2>();
    }

    public bool IsMoving()
    {
        return Movement.ReadValue<Vector2>().magnitude > 0;
    }

    public bool IsAiming()
    {
        return Aiming.ReadValue<Vector2>().magnitude > 0.01f ? true : false ||
             Aiming.ReadValue<Vector2>().magnitude < -0.01f ? true : false;
    }

    private void IsGamepad()
    {
        isGamepad = playerInput.currentControlScheme.Equals("Gamepad") ? true : false;
    }
    #endregion
}

