using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerController;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float SprintMultiplier;
    [SerializeField] private Transform Orientation;
    [SerializeField] private Rigidbody Rb;
    Vector3 MoveDirection;

    [Header("Ground Settings")]
    [SerializeField] private float PlayerHeight;
    [SerializeField] private LayerMask Ground;
    [SerializeField] private float GroundDrag;
    public bool IsGrounded;

    [Header("Jump Settings")]
    [SerializeField] private float JumpForce;
    [SerializeField] private float JumpCooldown;
    [SerializeField] private float AirMultiplier;
    [SerializeField] private float CoyoteTime;
    private float CoyoteTimeCtr;
    [SerializeField] private float JumpBufferTime;
    private float JumpBufferCtr;
    private bool ReadyToJump = true;

    [Header("Dash Settings")]
    [SerializeField] private float DashSpeed;
    [SerializeField] private float DashSpeedChangeFactor;
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

        SpeedControl();
        StateHandler();

        //Drag Handler
        if (IsGrounded) Rb.drag = GroundDrag;
        else Rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (State == MovementState.Dashing) return;

        //Coyote Time
        if (IsGrounded) CoyoteTimeCtr = CoyoteTime;
        else CoyoteTimeCtr -= Time.deltaTime;

        //Jump Buffer
        if (Input.GetKeyDown(KeyCode.Space)) JumpBufferCtr = JumpBufferTime;
        else JumpBufferCtr -= Time.deltaTime;

        MoveDirection = Orientation.forward * Input.GetAxisRaw("Vertical") + Orientation.right * Input.GetAxisRaw("Horizontal");
        if (IsGrounded) Rb.AddForce(MoveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);
        else if (!IsGrounded) Rb.AddForce(MoveDirection.normalized * MoveSpeed * 10f * AirMultiplier, ForceMode.Force);

        if (Input.GetKey(KeyCode.Space) && ReadyToJump && IsGrounded)
        {
            ReadyToJump = false;
            CoyoteTimeCtr = 0;
            JumpBufferCtr = 0;
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
