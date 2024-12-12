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

    private float GetHorizontal()
    {
        if(!IsOwner)
            return 0f;

        Keyboard keyboard = Keyboard.current;

        float horizontalInput = keyboard.dKey.isPressed ? 1 : keyboard.aKey.isPressed ? -1 : 0;

        return horizontalInput;
    }

    private float GetVertical()
    {
        if(!IsOwner)
            return 0f;

        Keyboard keyboard = Keyboard.current;

        float verticalInput = keyboard.wKey.isPressed ? 1 : keyboard.sKey.isPressed ? -1 : 0;

        return verticalInput;
    }

    private Vector3 GetDirection(float horizontalInput, float verticalInput)
    {
        Vector3 vertical = verticalInput * cameraMovement.orientation.transform.forward;
        Vector3 horizontal = horizontalInput * cameraMovement.orientation.transform.right;

        Vector3 moveDir = (vertical + horizontal).normalized;

        return moveDir;
    }

    private bool GetJump()
    {
        Keyboard keyboard = Keyboard.current;

        if (isGrounded && keyboard.spaceKey.wasPressedThisFrame)
            return true;
        else
            return false;
    }

    private void Jump(bool canJump)
    {
        if (canJump)
            rb.AddForce(transform.up * jumpSpeed, ForceMode.Impulse);
        //variable jump height
    }

    private void MoveForce(Vector3 direction)
    {
        rb.AddForce(direction * speed, ForceMode.Force);
    }

    private void Update()
    {
        direction = GetDirection(GetHorizontal(), GetVertical());
        canJump = GetJump();
    }

    private void FixedUpdate()
    {
        MoveForce(direction);
        Jump(canJump);
    }

    private void OnTriggerStay(Collider other)
    {
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
