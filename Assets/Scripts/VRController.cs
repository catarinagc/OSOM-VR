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
        moveProvider.enableFly = true;
        currentMode = MovementMode.Flying;
    }

    // Update is called once per frame
    //TODO meter botoes para interact bem feitos, estes sao placeholders
    void Update()
    {
        if (yButton.WasPressedThisFrame())
        {
            // pointerMode = InteractionMode.World;
            UI_Manager.CloseActiveUIs();
        }

        // if (pointerMode == InteractionMode.UI)
        //     return;

        HandleHotspotLook();

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
            if (currentHotspot)
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
            transform.position = initWalkPos.position;
            transform.rotation = initWalkPos.rotation;  
            currentMode = MovementMode.Walking;
        }
        else
        {
            moveProvider.enableFly = true;
            currentMode = MovementMode.Flying;
        }
    }

    private void HandleHotspotLook()
    {
        Ray ray = new Ray(rightController.position, rightController.forward);
        Debug.DrawRay(rightController.position, rightController.forward * 10f, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f)) // distance optional
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
