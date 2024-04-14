using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask Ground;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private Rigidbody Rb;
    [Space]

    [Header("Main Settings")]
    private float Gravity = -9.81f;
    [SerializeField] private float GravityMultiplier = 1f;
    public float MoveSpeed;
    [SerializeField] private float Speed;
    [SerializeField] private float DashSpeed;
    [SerializeField] private float DashSpeedChangeFactor;
    [SerializeField] private float SprintMultiplier = 2f;
    [SerializeField] private float JumpHeight;
    [Space]

    [Header("Other Settings")]
    private float TurnSmoothVelocity;
    [SerializeField] private float TurnSmoothTime;
    [SerializeField] private float CoyoteTime = 0.2f;
    private float CoyoteTimeCtr;
    [SerializeField] private float JumpBufferTime = 0.1f;
    private float JumpBufferCtr;
    [Space]
    private Vector3 Direction;
    public bool IsGrounded;
    public float VelocityY;
    private float TargetAngle;
    private float FinalAngle;
    private Vector3 MoveVector;

    private bool IsFalling;
    public bool IsDashing;

    public static event Action OnLand;
    public static event Action OnFall;
    public static event Action OnJump;

    public enum MovementState
    {
        Walking,
        Sprinting,
        Airborne,
        Dashing
    }
    public MovementState State;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        StateHandler();
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (State == MovementState.Dashing) return;

        IsGrounded = Physics.CheckSphere(GroundCheck.position, 0.1f, Ground);

        //Coyote Time
        if (IsGrounded) CoyoteTimeCtr = CoyoteTime;
        else CoyoteTimeCtr -= Time.deltaTime;

        //Jump Buffer
        if (Input.GetKeyDown(KeyCode.Space)) JumpBufferCtr = JumpBufferTime;
        else JumpBufferCtr -= Time.deltaTime;


        if (IsGrounded && VelocityY < 0f)
        {
            if (IsFalling)
            {
                OnLand?.Invoke();
                IsFalling = false;
            }
            VelocityY = -2f;
        }
        else
        {
            if (!IsFalling)
            {
                OnFall?.Invoke();
                IsFalling = true;
            }
            VelocityY += Gravity * GravityMultiplier * Time.deltaTime;
        }

        if (Direction.magnitude >= 0.1f)
        {
            TargetAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + PlayerCamera.eulerAngles.y;
            FinalAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref TurnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, FinalAngle, 0f);

            MoveVector = Quaternion.Euler(0f, TargetAngle, 0f) * Vector3.forward * MoveSpeed * (Input.GetKey(KeyCode.LeftShift) ? SprintMultiplier : 1);
            Rb.velocity = new Vector3(MoveVector.x, Rb.velocity.y, MoveVector.z);
        }

        if (JumpBufferCtr > 0 && CoyoteTimeCtr > 0)
        {
            OnJump?.Invoke();
            VelocityY = Mathf.Sqrt(JumpHeight * -1f * Gravity);
            CoyoteTimeCtr = 0;
            JumpBufferCtr = 0;
        }

        Rb.velocity = new Vector3(Rb.velocity.x, VelocityY, Rb.velocity.z);
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
            DesiredMoveSpeed = Speed * SprintMultiplier;
        }
        else if (IsGrounded)
        {
            State = MovementState.Walking;
            DesiredMoveSpeed = Speed;
        }
        else
        {
            State = MovementState.Airborne;
            if (DesiredMoveSpeed < (Speed * SprintMultiplier)) DesiredMoveSpeed = Speed;
            else DesiredMoveSpeed = Speed * SprintMultiplier;
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
