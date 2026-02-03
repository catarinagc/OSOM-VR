using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopController : MonoBehaviour
{
    [Header("Movement")]
    public InputActionReference moveAction;
    public float moveSpeed = 4f;

    [Header("Mouse Look")]
    public InputActionReference lookAction;
    public float mouseSensitivity = 0.1f; // Mouse Delta values are higher than stick values

    private float yaw;
    private float pitch;
    private Transform camTransform;

    void Awake()
    {
        // Finding the camera under the XR Origin hierarchy
        camTransform = Camera.main.transform;
        
        moveAction.action.Enable();
        if (lookAction != null) lookAction.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        // Move relative to where the body (this transform) is facing
        Vector3 move = transform.forward * input.y + transform.right * input.x;
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    private void HandleLook()
    {
        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

        // We rotate the entire object (this transform) based on mouse movement
        float yaw = mouseDelta.x * mouseSensitivity;
        float pitch = -mouseDelta.y * mouseSensitivity; // Negative to invert (Move mouse up = Look up)

        // Apply the rotation directly to the object
        // This makes 'transform.forward' point exactly where you are looking
        transform.Rotate(Vector3.up, yaw, Space.World);
        transform.Rotate(Vector3.right, pitch, Space.Self);
    }
}