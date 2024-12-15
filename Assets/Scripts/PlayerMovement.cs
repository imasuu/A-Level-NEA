using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    public CameraMovement cameraMovement;
    public Rigidbody rb;
    public float speed = 50f;
    public float jumpSpeed = 10f;

    private bool isGrounded = false;
    private Vector3 direction;
    private bool canJump;

    // Handle horizontal input from player
    private float GetHorizontal()
    {
        if (!IsOwner)
            return 0f;

        Keyboard keyboard = Keyboard.current;
        return keyboard.dKey.isPressed ? 1 : (keyboard.aKey.isPressed ? -1 : 0);
    }

    // Handle vertical input from player
    private float GetVertical()
    {
        if (!IsOwner)
            return 0f;

        Keyboard keyboard = Keyboard.current;
        return keyboard.wKey.isPressed ? 1 : (keyboard.sKey.isPressed ? -1 : 0);
    }

    // Get direction for movement based on inputs and camera orientation
    private Vector3 GetDirection(float horizontalInput, float verticalInput)
    {
        Vector3 vertical = verticalInput * cameraMovement.orientation.transform.forward;
        Vector3 horizontal = horizontalInput * cameraMovement.orientation.transform.right;

        return (vertical + horizontal).normalized;
    }

    // Check if the player wants to jump (only if the player owns the object)
    private bool GetJump()
    {
        if (!IsOwner || !isGrounded)
            return false;

        Keyboard keyboard = Keyboard.current;
        return keyboard.spaceKey.wasPressedThisFrame;
    }

    // Only the client that owns this object should send the jump request to the server
    [ServerRpc]
    private void JumpServerRpc()
    {
        rb.AddForce(transform.up * jumpSpeed, ForceMode.Impulse);
    }

    // Only the client with authority should request server to apply movement force
    [ServerRpc]
    private void MoveForceServerRpc(Vector3 direction)
    {
        rb.AddForce(direction * speed, ForceMode.Force);
    }

    // Update is run once per frame, to gather input.
    private void Update()
    {
        // Only allow input if we are the owner
        if (IsOwner)
        {
            direction = GetDirection(GetHorizontal(), GetVertical());
            canJump = GetJump();
        }
    }

    // FixedUpdate is run in sync with the physics engine.
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            // Request the server to apply movement and jumping forces.
            MoveForceServerRpc(direction);
            if (canJump)
            {
                JumpServerRpc();
            }
        }
    }

    // Trigger detection for ground checking
    private void OnTriggerStay(Collider other)
    {
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
