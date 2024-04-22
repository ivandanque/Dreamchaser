using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerController;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed;
    public float WalkSpeed;
    public float SprintMultiplier;
    public Transform Orientation;
    public Rigidbody Rb;
    Vector3 MoveDirection;

    [Header("Ground Settings")]
    public float PlayerHeight;
    public LayerMask Ground;
    public float GroundDrag;
    public bool IsGrounded;

    [Header("Jump Settings")]
    public float JumpForce;
    public float JumpCooldown;
    public float AirMultiplier;
    public float CoyoteTime;
    private float CoyoteTimeCtr;
    public float JumpBufferTime;
    private float JumpBufferCtr;
    private bool ReadyToJump = true;

    [Header("Dash Settings")]
    public float DashSpeed;
    public float DashSpeedChangeFactor;
    [HideInInspector] public float MaxYSpeed;
    public bool IsDashing;

    private enum MovementState
    {
        Walking,
        Sprinting,
        Dashing,
        Airborne
    }
    private MovementState State;

    private void Start()
    {
        
    }

    private void Update()
    {
        //Ground Check
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.1f, Ground);

        MovePlayer();
        SpeedControl();
        StateHandler();

        //Drag Handler
        if (IsGrounded) Rb.drag = GroundDrag;
        else Rb.drag = 0;
    }

    private void MovePlayer()
    {
        //Coyote Time
        if (IsGrounded) CoyoteTimeCtr = CoyoteTime;
        else CoyoteTimeCtr -= Time.deltaTime;

        //Jump Buffer
        if (Input.GetKeyDown(KeyCode.Space)) JumpBufferCtr = JumpBufferTime;

            
        else JumpBufferCtr -= Time.deltaTime;

        if (State == MovementState.Dashing) return;

        MoveDirection = Orientation.forward * Input.GetAxisRaw("Vertical") + Orientation.right * Input.GetAxisRaw("Horizontal");
        if (IsGrounded) Rb.AddForce(10f * MoveSpeed * MoveDirection.normalized, ForceMode.Force);
        else if (!IsGrounded) Rb.AddForce(10f * AirMultiplier * MoveSpeed * MoveDirection.normalized, ForceMode.Force);

        if (JumpBufferCtr > 0f && ReadyToJump && CoyoteTimeCtr > 0f)
        {
            ReadyToJump = false;
            CoyoteTimeCtr = 0f;
            JumpBufferCtr = 0f;
            Jump();
            Invoke(nameof(ResetJump), JumpCooldown);
        }
    }

    private void SpeedControl()
    {
        Vector3 FlatVel = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);

        if (FlatVel.magnitude > MoveSpeed)
        {
            Vector3 CappedVel = FlatVel.normalized * MoveSpeed;
            Rb.velocity = new Vector3(CappedVel.x, Rb.velocity.y, CappedVel.z);
        }

        if (MaxYSpeed != 0 && Rb.velocity.y > MaxYSpeed) Rb.velocity = new Vector3(Rb.velocity.x, MaxYSpeed, Rb.velocity.z);
    }

    private void Jump()
    {
        Rb.velocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);
        Rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        ReadyToJump = true;
    }

    private float DesiredMoveSpeed;
    private float LastDesiredMoveSpeed;
    private MovementState LastState;
    private bool KeepMomentum;

    private void StateHandler()
    {
        if (IsDashing)
        {
            State = MovementState.Dashing;
            DesiredMoveSpeed = DashSpeed;
            SpeedChangeFactor = DashSpeedChangeFactor;
        }
        else if (IsGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            State = MovementState.Sprinting;
            DesiredMoveSpeed = WalkSpeed * SprintMultiplier;
        }
        else if (IsGrounded)
        {
            State = MovementState.Walking;
            DesiredMoveSpeed = WalkSpeed;
        }
        else
        {
            State = MovementState.Airborne;
            if (DesiredMoveSpeed < (WalkSpeed * SprintMultiplier)) DesiredMoveSpeed = WalkSpeed;
            else DesiredMoveSpeed = WalkSpeed * SprintMultiplier;
        }

        bool DesiredMoveSpeedHasChanged = DesiredMoveSpeed != LastDesiredMoveSpeed;
        if (LastState == MovementState.Dashing) KeepMomentum = true;
        if (DesiredMoveSpeedHasChanged)
        {
            if (KeepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                MoveSpeed = DesiredMoveSpeed;
            }
        }

        LastDesiredMoveSpeed = DesiredMoveSpeed;
        LastState = State;
    }

    private float SpeedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(DesiredMoveSpeed - MoveSpeed);
        float startValue = MoveSpeed;

        float boostFactor = SpeedChangeFactor;

        while (time < difference)
        {
            MoveSpeed = Mathf.Lerp(startValue, DesiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        MoveSpeed = DesiredMoveSpeed;
        SpeedChangeFactor = 1f;
        KeepMomentum = false;
    }
}
