using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class DesktopController : MonoBehaviour
{
    [Header("Movement")]
    public InputActionReference moveAction;
    //public float moveSpeed = 4f;

    public enum MovementMode { Walking, Flying }

    [Header("Settings")]
    public MovementMode currentMode = MovementMode.Walking;
    public float moveSpeed = 5f;
    public float flySpeed = 10f;
    public float gravity = -9.81f;
    //public float mouseSensitivity = 0.1f;

    [Header("Mouse Look")]
    public InputActionReference lookAction;
    public float mouseSensitivity = 0.1f;

    private float pitch;
    public Transform camTransform;
    public Camera camera;
    private CharacterController controller;
    private float verticalVelocity;
    public Transform initWalkPos;
    private HotspotScript currentHotspot;

    public enum InteractionMode
    {
        World,
        UI
    }

    public InteractionMode pointerMode;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        moveAction.action.Enable();
        if (lookAction != null) lookAction.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pointerMode = InteractionMode.World;
    }

    void Update()
    {
        
        if(Keyboard.current.escapeKey.wasPressedThisFrame && pointerMode == InteractionMode.UI)
        {
            pointerMode = InteractionMode.World;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        if (pointerMode == InteractionMode.UI)
            return;
        
        HandleLook();
        HandleMovement();
        HandleHotspotLook();

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            changePosWalking();
            currentMode = (currentMode == MovementMode.Walking) ? MovementMode.Flying : MovementMode.Walking;
            verticalVelocity = 0; // Reset momentum when switching
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentHotspot != null)
            {
                pointerMode = InteractionMode.UI;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                currentHotspot.OnInteract();
            }
        }
    }

    private void HandleHotspotLook()
    {
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            HotspotScript hotspot = hit.collider.GetComponentInParent<HotspotScript>();

            if (hotspot != null)
            {
                if (currentHotspot != hotspot)
                {
                    if (currentHotspot != null)
                        currentHotspot.StopHover();

                    currentHotspot = hotspot;
                    currentHotspot.StartHover();
                }

                return;
            }
        }

        // If we are not looking at any hotspot
        if (currentHotspot != null)
        {
            currentHotspot.StopHover();
            currentHotspot = null;
        }
    }

    private void changePosWalking()
    {
        transform.position = initWalkPos.position;
        transform.rotation = initWalkPos.rotation;
    }

    private void HandleLook()
    {
        Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

        // Horizontal (Yaw)
        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);

        // Vertical (Pitch)
        pitch -= mouseDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Apply pitch to the camera only
        camTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDirection;

        if (currentMode == MovementMode.Flying)
        {
            // FLYING: Move exactly where the camera points
            moveDirection = (camTransform.forward * input.y) + (camTransform.right * input.x);
            controller.Move(moveDirection * flySpeed * Time.deltaTime);
        }
        else
        {
            // WALKING: Move only on the XZ plane (ignore camera tilt for direction)
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            moveDirection = (forward * input.y) + (right * input.x);

            // Apply Gravity
            if (controller.isGrounded && verticalVelocity < 0)
                verticalVelocity = -2f;
            else
                verticalVelocity += gravity * Time.deltaTime;

            Vector3 finalMove = (moveDirection * moveSpeed);
            finalMove.y = verticalVelocity;

            controller.Move(finalMove * Time.deltaTime);
        }
    }
}
