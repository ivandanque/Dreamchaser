using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform Orientation;
    [SerializeField] private Transform Player;
    [SerializeField] private Transform PlayerObject;
    [SerializeField] private Rigidbody Rb;

    [Header("Settings")]
    [SerializeField] private float RotationSpeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 ViewDirection = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        Orientation.forward = ViewDirection.normalized;

        Vector3 InputDirection = Orientation.forward * Input.GetAxis("Vertical") + Orientation.right * Input.GetAxis("Horizontal");
        if (InputDirection != Vector3.zero) PlayerObject.forward = Vector3.Slerp(PlayerObject.forward, InputDirection.normalized, Time.deltaTime * RotationSpeed);
    }
}
