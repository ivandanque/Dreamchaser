using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintMultiplier;
    public Transform orientation;
    public Rigidbody rb;
    Vector3 moveDirection;

    [Header("Ground Settings")]
    public float playerHeight;
    public LayerMask ground;
    public float groundDrag;
    public bool isGrounded;

    [Header("Jump Settings")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float coyoteTime;
    private float coyoteTimeCtr;
    public float jumpBufferTime;
    private float jumpBufferCtr;
    private bool readyToJump = true;

    [Header("Dash Settings")]
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    [HideInInspector] public float maxYSpeed;
    public bool isDashing;
    public bool isAttacking;
    public bool isPaused;

    private enum MovementState
    {
        Paused,
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
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, ground);

        MovePlayer();
        SpeedControl();
        StateHandler();

        //Drag Handler
        if (isGrounded) rb.drag = groundDrag;
        else rb.drag = 0;
    }

    private void MovePlayer()
    {
        //Coyote Time
        if (isGrounded) coyoteTimeCtr = coyoteTime;
        else coyoteTimeCtr -= Time.deltaTime;

        //Jump Buffer
        if (Input.GetKeyDown(KeyCode.Space)) jumpBufferCtr = jumpBufferTime;
        else jumpBufferCtr -= Time.deltaTime;

        if (State == MovementState.Dashing) return; //Don't move while dashing
        if (State == MovementState.Paused) return; //Don't move while paused

        moveDirection = orientation.forward * Input.GetAxisRaw("Vertical") + orientation.right * Input.GetAxisRaw("Horizontal");
        if (isGrounded) rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
        else if (!isGrounded) rb.AddForce(10f * airMultiplier * moveSpeed * moveDirection.normalized, ForceMode.Force);

        if (jumpBufferCtr > 0f && readyToJump && coyoteTimeCtr > 0f)
        {
            readyToJump = false;
            coyoteTimeCtr = 0f;
            jumpBufferCtr = 0f;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void SpeedControl()
    {
        Vector3 FlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (FlatVel.magnitude > moveSpeed)
        {
            Vector3 CappedVel = FlatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(CappedVel.x, rb.velocity.y, CappedVel.z);
        }

        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed) rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Unpause()
    {
        isPaused = false;
    }

    private float DesiredMoveSpeed;
    private float LastDesiredMoveSpeed;
    private MovementState LastState;
    private bool KeepMomentum;

    private void StateHandler()
    {
        if (isDashing)
        {
            State = MovementState.Dashing;
            DesiredMoveSpeed = dashSpeed;
            SpeedChangeFactor = dashSpeedChangeFactor;
        }
        else if (isPaused)
        {
            State = MovementState.Paused;
            DesiredMoveSpeed = 0f;
        }
        else if (isGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            State = MovementState.Sprinting;
            DesiredMoveSpeed = walkSpeed * sprintMultiplier;
        }
        else if (isGrounded)
        {
            State = MovementState.Walking;
            DesiredMoveSpeed = walkSpeed;
        }
        else
        {
            State = MovementState.Airborne;
            if (DesiredMoveSpeed < (walkSpeed * sprintMultiplier)) DesiredMoveSpeed = walkSpeed;
            else DesiredMoveSpeed = walkSpeed * sprintMultiplier;
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
                moveSpeed = DesiredMoveSpeed;
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
        float difference = Mathf.Abs(DesiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = SpeedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, DesiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = DesiredMoveSpeed;
        SpeedChangeFactor = 1f;
        KeepMomentum = false;
    }

    private void OnEnable()
    {
        PlayerAttackHandler.OnPlayerAttackStart += Pause;
        PlayerAttackHandler.OnPlayerAttackEnd += Unpause;
    }

    private void OnDisable()
    {
        PlayerAttackHandler.OnPlayerAttackEnd -= Unpause;
        PlayerAttackHandler.OnPlayerAttackStart -= Pause;
    }
}
