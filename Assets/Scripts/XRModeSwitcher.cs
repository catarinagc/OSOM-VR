using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;


public class XRModeSwitcher : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    public InputActionReference moveAction;
    private InputAction aButton;

    public GameObject xrOrigin;
    public GameObject desktopRig;
    public Transform initWalkPos;

    private bool isVRActive = false;
    private bool inFlightMode = true;
    private bool isOn;

    void Awake()
    {
        //aButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("AButton");
        //aButton.Enable();
    }

    void Start()
    {
        //bool vrActive = XRSettings.isDeviceActive;
        //neste ponto ainda não ativa, apenas no update, ver se há uma forma melhor

        //xrOrigin.SetActive(vrActive);
        //desktopRig.SetActive(!vrActive);

        //Debug.Log("XR Active: " + XRSettings.isDeviceActive);
        //if (!XRSettings.isDeviceActive)
        //{
        //    EnableDesktopMaps();
        //}

    }

    void Update()
    {
        if (XRSettings.isDeviceActive)
        {
            xrOrigin.SetActive(true);
            desktopRig.SetActive(false);
        }
        else
        {
            xrOrigin.SetActive(false);
            desktopRig.SetActive(true);
            EnableDesktopMaps();
        }
        //if (aButton.WasPressedThisFrame())
        //{
        //    OnAPressed();
        //}

        //if(inFlightMode)

        //inputActions.XRI_Right.AButton.performed += ctx =>
        //{
        //    Debug.Log("A button pressed");
        //};
    }

    private void OnAPressed()
    {
        isOn = !isOn;

        //Debug.Log("Toggle state: " + isOn);

        // Do your thing here
        // Example:
        // xrRayInteractor.enabled = isOn;
        //if (!isVRActive)
        //{
        //    // 1. Get the CharacterController component
        //    CharacterController controller = desktopRig.GetComponent<CharacterController>();

        //    if (controller != null)
        //    {
        //        // 2. Disable it temporarily to 'teleport'
        //        controller.enabled = false;

        //        // 3. Set the position (assuming initWalkPos is a Transform)
        //        desktopRig.transform.position = initWalkPos.position;
        //        desktopRig.transform.rotation = initWalkPos.rotation;

        //        // 4. Re-enable it
        //        controller.enabled = true;
        //    }
        //}
    }

    void EnableDesktopMaps()
    {
        // Find and enable the locomotion maps
        var leftLocomotion = inputActions.FindActionMap("XRI Left Locomotion");
        var rightLocomotion = inputActions.FindActionMap("XRI Right Locomotion");

        if (leftLocomotion != null) leftLocomotion.Enable();
        if (rightLocomotion != null) rightLocomotion.Enable();

        //Debug.Log("Desktop input maps enabled");
    }
}
