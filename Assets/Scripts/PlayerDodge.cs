using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [Header("References")]
    public Transform Orientation;
    public Transform PlayerCamera;
    private Rigidbody Rb;
    private PlayerMovement PM;

    [Header("Dash Settings")]
    [SerializeField] private float DashForce;
    [SerializeField] private float DashUpwardForce;
    [SerializeField] private float DashUpwardSpeedCap;
    [SerializeField] private float DashTime;
    [SerializeField] private float DashCooldown;

    [Header("More Dash Settings")]
    [SerializeField] private bool UseCameraForward = true;
    [SerializeField] private bool AllowAllDirections = true;
    [SerializeField] private bool DisableGravity = false;
    [SerializeField] private bool ResetVelocity = true;

    private float DashCooldownTimer;
    private Vector3 DelayedForceToApply;

    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
        PM = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) Dash();

        if (DashCooldownTimer > 0) DashCooldownTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        if (DashCooldownTimer > 0) return;
        else DashCooldownTimer = DashCooldown;

        PM.IsDashing = true;
        PM.MaxYSpeed = DashUpwardSpeedCap;

        Transform ForwardT;
        if (UseCameraForward) ForwardT = PlayerCamera;
        else ForwardT = Orientation;

        Vector3 direction = GetDirection(ForwardT);

        Vector3 ForceToApply = direction * DashForce + Orientation.up * DashUpwardForce;
        if (DisableGravity) Rb.useGravity = false;
        DelayedForceToApply = ForceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), DashTime);
    }

    private void DelayedDashForce()
    {
        if (ResetVelocity) Rb.velocity = Vector3.zero;
        Rb.AddForce(DelayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        PM.IsDashing = false;
        PM.MaxYSpeed = 0;
        if (DisableGravity) Rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform ForwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (AllowAllDirections) direction = ForwardT.forward * verticalInput + ForwardT.right * horizontalInput;
        else direction = ForwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = ForwardT.forward;

        return direction.normalized;
    }
}
