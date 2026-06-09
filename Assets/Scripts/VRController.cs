using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Collections;
public class VRController : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] UI_Manager UI_Manager;
    public DynamicMoveProvider moveProvider;
    public enum MovementMode { Walking, Flying }
    public MovementMode currentMode;
    public Transform initWalkPos;
    private InputAction aButton;
    private InputAction bButton;
    private InputAction yButton;
    private InputAction xButton;
    private InputAction rightPress;
    private InputAction leftPress;
    private HotspotScript currentHotspot;
    public Transform rightController;
    public Transform leftControllerTransform;
    public GameObject leftController;
    public Screenshot screenshot;
    [SerializeField] private float maxHeight = 3.0f;
    [SerializeField] private Transform xrOrigin;
    // Add these fields at the top of your class
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float accelerationTime = 3f; // seconds to reach max speed
    [SerializeField] private float decelerationTime = 0.5f; // seconds to return to base speed

    [Header("Auto-Move")]
    [SerializeField] private float autoMoveSpeed = 5f;
    [SerializeField] private float arrivalThreshold = 0.5f;
    public ControllerManager controllerManager;
    private Coroutine _autoMoveCoroutine;
    private float currentSpeedMultiplier = 1f;
    private InputAction moveAction;

    public enum InteractionMode
    {
        World,
        UI
    }
    public InteractionMode pointerMode;
    void Awake()
    {
        moveAction = inputActions.FindActionMap("XRI Left Locomotion").FindAction("Move");
        moveAction.Enable();
        aButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("AButton");
        bButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("BButton");
        rightPress = inputActions.FindActionMap("XRI Right Interaction").FindAction("UI Press");
        leftPress = inputActions.FindActionMap("XRI Left Interaction").FindAction("UI Press");
        yButton = inputActions.FindActionMap("XRI Left Interaction").FindAction("YButton");
        xButton = inputActions.FindActionMap("XRI Left Interaction").FindAction("XButton");
        aButton.Enable();
        bButton.Enable();
        yButton.Enable();
        xButton.Enable();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveProvider.enableFly = false;
        currentMode = MovementMode.Walking;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (yButton.WasPressedThisFrame())
    //     {
    //         UI_Manager.CloseActiveUIs();
    //     }

    //     if (!UI_Manager.isHotspotActive())
    //     {
    //         HandleHotspotLook();
    //     }

    //     if (aButton.WasPressedThisFrame())
    //     {
    //         screenshot.TakeScreenshotVR();
    //     }

    //     if (xButton.WasPressedThisFrame())
    //     {
    //         UI_Manager.OpenMenu();
    //     }

    //     if (bButton.WasPressedThisFrame())
    //     {
    //         toggleFly();
    //     }

    //     if (rightPress.WasPressedThisFrame() || leftPress.WasPressedThisFrame())
    //     {
    //         if (currentHotspot && !UI_Manager.isHotspotActive())
    //             currentHotspot.OnInteract();
    //     }
    // }

    // Replace your Update() with this:
    void Update()
    {
        HandleMovementSpeed();

        if (yButton.WasPressedThisFrame())
            UI_Manager.CloseActiveUIs();

        if (!UI_Manager.isHotspotActive())
            HandleHotspotLook();

        if (aButton.WasPressedThisFrame())
            screenshot.TakeScreenshotVR();

        if (xButton.WasPressedThisFrame())
            controllerManager.MoveToHomePosition();

        if (bButton.WasPressedThisFrame())
            toggleFly();

        if (rightPress.WasPressedThisFrame() || leftPress.WasPressedThisFrame())
        {
            if (currentHotspot && !UI_Manager.isHotspotActive())
                currentHotspot.OnInteract();
        }
    }

    public void TakeScreenhot()
    {
        screenshot.TakeScreenshotVR();
    }

    private void HandleMovementSpeed()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        bool isMoving = moveInput.magnitude > 0.1f;

        if (isMoving)
        {
            // Accelerate toward max multiplier
            float targetMultiplier = maxSpeed / baseSpeed;
            currentSpeedMultiplier = Mathf.MoveTowards(
                currentSpeedMultiplier,
                targetMultiplier,
                (targetMultiplier - 1f) / accelerationTime * Time.deltaTime
            );
        }
        else
        {
            // Decelerate back to base
            currentSpeedMultiplier = Mathf.MoveTowards(
                currentSpeedMultiplier,
                1f,
                (maxSpeed / baseSpeed - 1f) / decelerationTime * Time.deltaTime
            );
        }

        moveProvider.moveSpeed = baseSpeed * currentSpeedMultiplier;
    }

    public float GetHeight()
    {
        return gameObject.transform.position.y;
    }

    public void toggleFly()
    {
        if (currentMode == MovementMode.Flying)
        {
            moveProvider.enableFly = false;

            currentMode = MovementMode.Walking;

            SnapToGround();
        }
        else
        {
            moveProvider.enableFly = true;

            currentMode = MovementMode.Flying;
        }
    }

    private void SnapToGround()
    {
        float raycastOriginHeight = 10f; // cast from above in case we're clipping
        Vector3 rayOrigin = xrOrigin.position + Vector3.up * raycastOriginHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f))
        {
            Vector3 snappedPos = xrOrigin.position;
            snappedPos.y = hit.point.y;
            xrOrigin.position = snappedPos;
        }
    }

    // private void HandleHotspotLook()
    // {
    //     Ray ray = new Ray(rightController.position, rightController.forward);
    //     Debug.DrawRay(rightController.position, rightController.forward * 10f, Color.red);
    //     RaycastHit hit;

    //     if (Physics.Raycast(ray, out hit, 10f))
    //     {
    //         // If the first thing hit is UI, stop here
    //         if (hit.collider.CompareTag("UI"))
    //         {
    //             if (currentHotspot != null)
    //             {
    //                 currentHotspot.StopHover();
    //                 currentHotspot = null;
    //             }
    //             return;
    //         }

    //         HotspotScript hotspot = hit.collider.GetComponentInParent<HotspotScript>();
    //         if (hotspot != null)
    //         {
    //             if (currentHotspot != hotspot)
    //             {
    //                 if (currentHotspot != null)
    //                     currentHotspot.StopHover();

    //                 currentHotspot = hotspot;
    //                 currentHotspot.StartHover();
    //             }
    //             return;
    //         }
    //     }

    //     if (currentHotspot != null)
    //     {
    //         currentHotspot.StopHover();
    //         currentHotspot = null;
    //     }
    // }

    private void HandleHotspotLook()
    {
        // Check both controllers, use whichever hits a hotspot (right takes priority)
        Transform[] controllers = { rightController, leftControllerTransform };

        HotspotScript foundHotspot = null;

        foreach (Transform controller in controllers)
        {
            Ray ray = new Ray(controller.position, controller.forward);
            Debug.DrawRay(controller.position, controller.forward * 10f, Color.red);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10f))
            {
                // If the first thing hit is UI, skip this controller
                if (hit.collider.CompareTag("UI"))
                    continue;

                HotspotScript hotspot = hit.collider.GetComponentInParent<HotspotScript>();
                if (hotspot != null)
                {
                    foundHotspot = hotspot;
                    break; // Found one, no need to check the other controller
                }
            }
        }

        // Update hover state based on what was found
        if (foundHotspot != null)
        {
            if (currentHotspot != foundHotspot)
            {
                if (currentHotspot != null)
                    currentHotspot.StopHover();

                currentHotspot = foundHotspot;
                currentHotspot.StartHover();
            }
        }
        else
        {
            if (currentHotspot != null)
            {
                currentHotspot.StopHover();
                currentHotspot = null;
            }
        }
    }

    public void stopInteraction()
    {
        var radial = leftController.GetComponentInChildren<RadialSelection>();

        if (radial == null)
            return;

        Destroy(radial.gameObject);
    }

    public void MoveToHotspot(HotspotScript hotspot)
    {
        if (_autoMoveCoroutine != null)
            StopCoroutine(_autoMoveCoroutine);

        _autoMoveCoroutine = StartCoroutine(AutoMoveCoroutine(hotspot.currentGlobalPosition));
    }

    public void MoveToHomePosition(Vector3 homePos)
    {
        if (_autoMoveCoroutine != null)
            StopCoroutine(_autoMoveCoroutine);

        _autoMoveCoroutine = StartCoroutine(AutoMoveCoroutine(homePos));
    }

    private IEnumerator AutoMoveCoroutine(Vector3 target)
    {
        while (true)
        {
            // Abort if the player touches the thumbstick
            Vector2 input = moveAction.ReadValue<Vector2>();
            if (input.magnitude > 0.1f)
            {
                _autoMoveCoroutine = null;
                yield break;
            }

            Vector3 current = xrOrigin.position;
            float distance = Vector3.Distance(current, target);

            if (distance < arrivalThreshold)
            {
                _autoMoveCoroutine = null;
                yield break;
            }

            // Move the XR Origin directly toward the target
            Vector3 direction = (target - current).normalized;
            xrOrigin.position = Vector3.MoveTowards(
                current,
                target,
                autoMoveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }
}
