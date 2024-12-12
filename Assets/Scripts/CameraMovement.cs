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
        if(!IsOwner)
            return;

        Vector3 rot = transform.localRotation.eulerAngles;
        float yaw = mouseMovement.x * sensitivity * sensMultiplier * Time.deltaTime % 360f;
        float pitch = mouseMovement.y * sensitivity * sensMultiplier * Time.deltaTime % 360f;

        desiredX = rot.y + yaw;
        xRotation -= pitch;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //reset mouse movement when not moving
        mouseMovement = Vector2.zero;
    }

    private void Update()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        MouseMovement();

        transform.position = head.position;
        orientation.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void LateUpdate()
    {
        transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
    }
}
