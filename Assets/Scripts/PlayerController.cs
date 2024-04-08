using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask Ground;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private Rigidbody Rb;
    [Space]
    private float Gravity = -9.81f;
    [SerializeField] private float GravityMultiplier = 1f;
    [SerializeField] private float Speed;
    [SerializeField] private float JumpHeight;
    [Space]
    private float TurnSmoothVelocity;
    [SerializeField] private float TurnSmoothTime;

    private Vector3 Direction;
    public bool IsGrounded;
    private float VelocityY;
    private float TargetAngle;
    private float FinalAngle;
    private Vector3 MoveVector;

    private bool IsFalling;

    public static event Action OnLand;
    public static event Action OnFall;
    public static event Action OnJump;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        MovePlayer();
    }

    private void MovePlayer()
    {
        IsGrounded = Physics.CheckSphere(GroundCheck.position, 0.1f, Ground);

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

            MoveVector = Quaternion.Euler(0f, TargetAngle, 0f) * Vector3.forward * Speed;
            Rb.velocity = new Vector3(MoveVector.x, Rb.velocity.y, MoveVector.z);
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            OnJump?.Invoke();
            VelocityY = Mathf.Sqrt(JumpHeight * -1f * Gravity);
        }

        Rb.velocity = new Vector3(Rb.velocity.x, VelocityY, Rb.velocity.z);
    }
}
