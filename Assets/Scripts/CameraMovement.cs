using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;

public class CameraMovement : NetworkBehaviour
{
    public Transform orientation;
    public Transform head;
    public float sensitivity = 3f;

    private InputAction action;
    private Vector2 mouseMovement;

    private float sensMultiplier = 1f;
    private float xRotation;
    private float desiredX;

    private void OnEnable()
    {
        action = new InputAction("Mouse Movement", binding: "<Mouse>/delta");
        action.performed += ctx => mouseMovement = ctx.ReadValue<Vector2>();

        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    private void MouseMovement()
    {
        // Ensure this is only applied to the local player
        if (!IsOwner)
            return;

        Vector3 rot = transform.localRotation.eulerAngles;
        float yaw = mouseMovement.x * sensitivity * sensMultiplier * Time.deltaTime % 360f;
        float pitch = mouseMovement.y * sensitivity * sensMultiplier * Time.deltaTime % 360f;

        desiredX = rot.y + yaw;
        xRotation -= pitch;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Reset mouse movement after processing it
        mouseMovement = Vector2.zero;
    }

    private void Update()
    {
        // Only the local player should manipulate the camera and cursor
        if (!IsOwner)
            return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        MouseMovement();

        // Update head tracking and orientation for the local player
        transform.position = head.position;
        orientation.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void LateUpdate()
    {
        // Local camera rotation
        if (IsOwner)
        {
            transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        }
    }
}
