using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

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
    private HotspotScript currentHotspot;
    public Transform rightController;
    public GameObject leftController;
    public Screenshot screenshot;
    [SerializeField] private float maxHeight = 3.0f;
    [SerializeField] private Transform xrOrigin;

    public enum InteractionMode
    {
        World,
        UI
    }
    public InteractionMode pointerMode;
    void Awake()
    {
        aButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("AButton");
        bButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("BButton");
        rightPress = inputActions.FindActionMap("XRI Right Interaction").FindAction("UI Press");
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
    void Update()
    {
        if (yButton.WasPressedThisFrame())
        {
            UI_Manager.CloseActiveUIs();
        }

        if (!UI_Manager.isHotspotActive())
        {
            HandleHotspotLook();
        }

        if (aButton.WasPressedThisFrame())
        {
            screenshot.TakeScreenshotVR();
        }

        if (xButton.WasPressedThisFrame())
        {
            UI_Manager.OpenMenu();
        }

        if (bButton.WasPressedThisFrame())
        {
            toggleFly();
        }

        if (rightPress.WasPressedThisFrame())
        {
            if (currentHotspot && !UI_Manager.isHotspotActive())
                currentHotspot.OnInteract();
        }
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

    private void HandleHotspotLook()
    {
        Ray ray = new Ray(rightController.position, rightController.forward);
        Debug.DrawRay(rightController.position, rightController.forward * 10f, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            // If the first thing hit is UI, stop here
            if (hit.collider.CompareTag("UI"))
            {
                if (currentHotspot != null)
                {
                    currentHotspot.StopHover();
                    currentHotspot = null;
                }
                return;
            }

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

        if (currentHotspot != null)
        {
            currentHotspot.StopHover();
            currentHotspot = null;
        }
    }

    public void stopInteraction()
    {
        var radial = leftController.GetComponentInChildren<RadialSelection>();

        if (radial == null)
            return;

        Destroy(radial.gameObject);
    }
}
