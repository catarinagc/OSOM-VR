using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
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
    public HotspotScript currentHotspot;
    [SerializeField] Screenshot screenshot;
    public GameObject anotationPanel;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float accelerationTime = 3f;
    [SerializeField] private float decelerationTime = 0.5f;

    private float currentSpeedMultiplier = 1f;
    [SerializeField] UI_Manager UI_Manager;
    [SerializeField] BreakwaterZoneManager breakwaterZoneManager;
    [Header("Interaction")]
    public float maxInteractDistance = 5f;
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

        if (lookAction) 
            lookAction.action.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pointerMode = InteractionMode.World;
    }

    void Update()
    {
        if (!UI_Manager.isHotspotActive())
        {
            HandleMovement();
            HandleHotspotLook();

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                HandleLook();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (currentHotspot != null)
            {
                currentHotspot.OnInteract();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                currentHotspot.StopHover();
                currentHotspot = null;
            }

            // if (Cursor.lockState == CursorLockMode.None && EventSystem.current.IsPointerOverGameObject())
            //     return;

        }
        
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UI_Manager.CloseActiveUIs();
        }

        if(Keyboard.current.hKey.wasPressedThisFrame)
        {
            UI_Manager.OpenHotspotChangeMenu();
        }

        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!anotationPanel.active)
            {
                UI_Manager.OpenMenu();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            TakeScreenhot();
        }

        if(Keyboard.current.zKey.wasPressedThisFrame && breakwaterZoneManager.GetHasSelection())
        {
            UI_Manager.OpenZoneMenu();
        }
    
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ChangeMovementMode();
        }
    }

    public void ChangeMovementMode()
    {
        if (currentMode == MovementMode.Flying)
        {
            currentMode = MovementMode.Walking;
            verticalVelocity = 0;
            SnapToGround();
        }
        else
        {
            currentMode = MovementMode.Flying;
            verticalVelocity = 0;
        }
    }

    public void TakeScreenhot()
    {
        screenshot.TakeScreenshot();
    }

    public float GetHeight()
    {
        return gameObject.transform.position.y;
    }

    private void HandleHotspotLook()
    {
        //Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = camera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxInteractDistance))
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

    [SerializeField] private float groundSnapRayOriginHeight = 2f;

    private void SnapToGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * groundSnapRayOriginHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f))
        {
            controller.enabled = false;
            Vector3 snappedPos = transform.position;
            snappedPos.y = hit.point.y;
            transform.position = snappedPos;
            controller.enabled = true;
        }
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

    // void HandleMovement()
    // {
    //     Vector2 input = moveAction.action.ReadValue<Vector2>();
    //     Vector3 moveDirection;

    //     if (currentMode == MovementMode.Flying)
    //     {
    //         Vector3 move = (camTransform.forward * input.y) + (camTransform.right * input.x);
    //         Vector3 proposed = transform.position + (move * flySpeed * Time.deltaTime);

    //         // only apply Y if under max height
    //         controller.Move(move * flySpeed * Time.deltaTime);

    //     }
    //     else
    //     {
    //         Vector3 forward = transform.forward;
    //         Vector3 right = transform.right;
    //         moveDirection = (forward * input.y) + (right * input.x);

    //         // Apply Gravity
    //         if (controller.isGrounded && verticalVelocity < 0)
    //             verticalVelocity = -2f;
    //         else
    //             verticalVelocity += gravity * Time.deltaTime;

    //         Vector3 finalMove = (moveDirection * moveSpeed);
    //         finalMove.y = verticalVelocity;

    //         controller.Move(finalMove * Time.deltaTime);
    //     }
    // }

    void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        bool isMoving = input.magnitude > 0.1f;

        // Ramp speed up while moving, reset when stopped
        if (isMoving)
        {
            float targetMultiplier = maxSpeed / moveSpeed;
            currentSpeedMultiplier = Mathf.MoveTowards(
                currentSpeedMultiplier,
                targetMultiplier,
                (targetMultiplier - 1f) / accelerationTime * Time.deltaTime
            );
        }
        else if (_autoMoveCoroutine == null) // only reset if not auto-moving
        {
            currentSpeedMultiplier = 1f;
        }

        float currentSpeed = moveSpeed * currentSpeedMultiplier;
        float currentFlySpeed = flySpeed * currentSpeedMultiplier;

        if (currentMode == MovementMode.Flying)
        {
            Vector3 move = (camTransform.forward * input.y) + (camTransform.right * input.x);
            controller.Move(move * currentFlySpeed * Time.deltaTime);
        }
        else
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            Vector3 moveDirection = (forward * input.y) + (right * input.x);

            if (controller.isGrounded && verticalVelocity < 0)
                verticalVelocity = -2f;
            else
                verticalVelocity += gravity * Time.deltaTime;

            Vector3 finalMove = (moveDirection * currentSpeed);
            finalMove.y = verticalVelocity;

            controller.Move(finalMove * Time.deltaTime);
        }
    }

    [Header("Auto-Move")]
    [SerializeField] private float autoMoveSpeed = 5f;
    [SerializeField] private float arrivalThreshold = 0.5f;

    private Coroutine _autoMoveCoroutine;

    public void MoveToHotspot(HotspotScript hotspot)
    {
        if (_autoMoveCoroutine != null)
            StopCoroutine(_autoMoveCoroutine);

        _autoMoveCoroutine = StartCoroutine(AutoMoveCoroutine(hotspot.currentGlobalPosition));
    }

    private Vector3 lastPos;

    private IEnumerator AutoMoveCoroutine(Vector3 target)
    {
        int stuckFrames = 0;
        const int stuckThreshold = 4;
        const float stuckMoveThreshold = 0.001f;
        float nudgeDirection = 1f;
        int nudgeFramesRemaining = 0;
        const int nudgeDuration = 6;
        const float nudgeStrength = 0.4f;
        while (true)
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            if (input.magnitude > 0.1f)
            {
                currentSpeedMultiplier = 1f;
                _autoMoveCoroutine = null;
                yield break;
            }

            Vector3 current = transform.position;
            float distance = Vector3.Distance(current, target);

            if (distance < arrivalThreshold)
            {
                currentSpeedMultiplier = 1f;
                _autoMoveCoroutine = null;
                yield break;
            }

            // Ramp up the same way HandleMovement does
            float targetMultiplier = maxSpeed / moveSpeed;
            currentSpeedMultiplier = Mathf.MoveTowards(
                currentSpeedMultiplier,
                targetMultiplier,
                (targetMultiplier - 1f) / accelerationTime * Time.deltaTime
            );
            Debug.Log(currentSpeedMultiplier);


            Vector3 direction = (target - current).normalized;
            Vector3 move = direction * moveSpeed * currentSpeedMultiplier;
            if (lastPos != null && Vector3.Distance(transform.position, lastPos) < stuckMoveThreshold)
            {
                stuckFrames++;
                if (stuckFrames >= stuckThreshold)
                {
                    nudgeFramesRemaining = nudgeDuration;
                    nudgeDirection = (stuckFrames / stuckThreshold % 2 == 0) ? 1f : -1f;
                    stuckFrames = 0;
                }
            }
            else
            {
                stuckFrames = 0;
            }

            // Apply nudge perpendicular to movement direction
            if (nudgeFramesRemaining > 0)
            {
                Vector3 perpendicular = new Vector3(-direction.z, 0f, direction.x);
                move += perpendicular * nudgeStrength * moveSpeed * nudgeDirection;
                nudgeFramesRemaining--;
            }

            controller.Move(move * Time.deltaTime);
            lastPos = transform.position;
            yield return null;
        }
    }

    public void MoveToHomePosition(Vector3 homePos)
    {
        if (_autoMoveCoroutine != null)
            StopCoroutine(_autoMoveCoroutine);

        currentMode = MovementMode.Flying;
        verticalVelocity = 0;
        _autoMoveCoroutine = StartCoroutine(AutoMoveCoroutine(homePos));
    }
}
