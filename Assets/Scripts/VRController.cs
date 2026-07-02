using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
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
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort.TunnelingVignetteController vignetteController;
    public ControllerManager controllerManager;
    private Coroutine _autoMoveCoroutine;
    private float currentSpeedMultiplier = 1f;
    private InputAction moveAction;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider teleportationProvider;
    [SerializeField] private CurveVisualController rightCurveVisual;
    [SerializeField] private CurveVisualController leftCurveVisual;
    [SerializeField] private float interactDistance = 10f;
    [SerializeField] private float defaultRayDistance = 0.2f;
    [SerializeField] Vector3 hotspotOffset = new Vector3(-10,0,-10);
    private bool foundHotspotWithLeftController = false;
    [SerializeField] GameObject homePos;

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
        //MoveToHomePosition(homePos.transform.position);
        //SetPositionDirect(homePos.transform.position, homePos.transform.rotation);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveProvider.enableFly = true;
        currentMode = MovementMode.Flying;
        UI_Manager.setMovementModeIsFly(currentMode == MovementMode.Flying);
        TelemetryLogger.Instance.LogUIInteraction("Fly");
    }

    // Replace your Update() with this:
    void Update()
    {
        Debug.Log("HOTSPOT " + UI_Manager.isHotspotActive());
        HandleMovementSpeed();

        if (yButton.WasPressedThisFrame())
            UI_Manager.CloseActiveUIs();

        if (!UI_Manager.isHotspotActive())
            HandleHotspotLook();

        if (aButton.WasPressedThisFrame())
        {
            screenshot.TakeScreenshotVR();
            TelemetryLogger.Instance.LogUIInteraction("Screenshot", "Shortcut");
        }

        if (xButton.WasPressedThisFrame())
        {
            UI_Manager.OpenMenu();
            TelemetryLogger.Instance.LogUIInteraction("Open Menu", "Shortcut");
        }

        if (bButton.WasPressedThisFrame())
        {
            toggleFly();
            TelemetryLogger.Instance.LogUIInteraction("Movement Mode", "Shortcut");
        }

        if (rightPress.WasPressedThisFrame() || leftPress.WasPressedThisFrame())
        {
            if (currentHotspot && !UI_Manager.isHotspotActive())
            {
                if (rightPress.WasPressedThisFrame() && foundHotspotWithLeftController == true)
                    return;
                if(leftPress.WasPressedThisFrame() && foundHotspotWithLeftController == false)
                    return;
                
                currentHotspot.OnInteract();
                currentHotspot.StopHover();
                currentHotspot = null;
                CurveVisualController[] visuals = { rightCurveVisual, leftCurveVisual };
                for (int i = 0; i < visuals.Length; i++)
                {
                    if (visuals[i])
                        visuals[i].restingVisualLineLength = defaultRayDistance;
                }
            }
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
        else if (_autoMoveCoroutine == null) // only reset if not auto-moving
        {
            currentSpeedMultiplier = 1f;
        }

        moveProvider.moveSpeed = baseSpeed * currentSpeedMultiplier;
    }

    public float GetHeight()
    {
        return gameObject.transform.position.y;
    }

    public Vector3 GetPosition()
    {
        return xrOrigin.position;
    }

    public void toggleFly()
    {
        if (currentMode == MovementMode.Flying)
        {
            moveProvider.enableFly = false;

            currentMode = MovementMode.Walking;

            SnapToGround();
            UI_Manager.setMovementModeIsFly(false);
            TelemetryLogger.Instance.LogUIInteraction("Walk");
        }
        else
        {
            moveProvider.enableFly = true;

            currentMode = MovementMode.Flying;
            UI_Manager.setMovementModeIsFly(true);
            TelemetryLogger.Instance.LogUIInteraction("Fly");
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


    private void HandleHotspotLook()
    {
        Transform[] controllers = { rightController, leftControllerTransform };
        CurveVisualController[] visuals = { rightCurveVisual, leftCurveVisual };

        HotspotScript foundHotspot = null;
        int foundIndex = -1;
        float foundDistance = defaultRayDistance;

        for (int i = 0; i < controllers.Length; i++)
        {
            Transform controller = controllers[i];
            Ray ray = new Ray(controller.position, controller.forward);
            Debug.DrawRay(controller.position, controller.forward * 10f, Color.red);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.CompareTag("UI")) continue;

                HotspotScript hotspot = hit.collider.GetComponentInParent<HotspotScript>();
                if (hotspot != null)
                {
                    foundHotspot = hotspot;
                    foundIndex = i;
                    foundDistance = hit.distance;
                    foundHotspotWithLeftController = (i == 1);
                    break;
                }
            }
        }

        // Update hover state
        if (foundHotspot != null)
        {
            if (currentHotspot != foundHotspot)
            {
                if (currentHotspot != null) currentHotspot.StopHover();
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

        // Only extend the ray that's pointing at the hotspot
        for (int i = 0; i < visuals.Length; i++)
        {
            if (visuals[i])
                visuals[i].restingVisualLineLength = (i == foundIndex) ? foundDistance : defaultRayDistance;
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
        TeleportTo(hotspot.currentGlobalPosition + hotspotOffset, hotspot.currentGlobalPosition);
    }

    public void MoveToHomePosition(Vector3 homePos)
    {
        if (currentMode == MovementMode.Walking)
            toggleFly();
        TeleportTo(homePos);
    }

    private void TeleportTo(Vector3 targetPosition, Vector3? lookAtPosition = null)
    {
        var request = new UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportRequest
        {
            destinationPosition = targetPosition,
            matchOrientation = UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.MatchOrientation.None
        };

        if (lookAtPosition.HasValue)
        {
            Vector3 flatDirection = lookAtPosition.Value - targetPosition;
            flatDirection.y = 0f;

            if (flatDirection.sqrMagnitude > 0.0001f)
            {
                request.destinationRotation = Quaternion.LookRotation(flatDirection.normalized, Vector3.up);
                request.matchOrientation = UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.MatchOrientation.TargetUpAndForward;
            }
        }


        teleportationProvider.QueueTeleportRequest(request);
    }

    // private void SetPositionDirect(Vector3 targetPosition, Vector3? lookAtPosition = null)
    // {
    //     xrOrigin.position = targetPosition;

    //     if (lookAtPosition.HasValue)
    //     {
    //         Vector3 flatDirection = lookAtPosition.Value - targetPosition;
    //         flatDirection.y = 0f;

    //         if (flatDirection.sqrMagnitude > 0.0001f)
    //         {
    //             xrOrigin.rotation = Quaternion.LookRotation(flatDirection.normalized, Vector3.up);
    //         }
    //     }
    // }

    private void SetPositionDirect(Vector3 targetPosition, Quaternion targetRotation)
    {
        xrOrigin.position = targetPosition;
        xrOrigin.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
    }
}
