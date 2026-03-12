using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Collections;


public class XRModeSwitcher : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    public InputActionReference moveAction;
    private InputAction aButton;

    public GameObject xrOrigin;
    public GameObject desktopRig;
    public Transform initWalkPos;
    [SerializeField] GameObject desktop_view;
    [SerializeField]  public UI_Manager desktop_view_image_manager;
    [SerializeField] public UI_Manager vr_view_image_manager;
    [SerializeField] public HotspotManager hotspotManager;
    private bool isVRActive = false;
    private bool inFlightMode = true;
    private bool isOn;
    public static event Action<bool> OnModeSelected; // bool = isVR

    void Awake()
    {
        StartCoroutine(CheckXR());
    }

    private IEnumerator CheckXR()
    {
        // 1. Initialize XR Loader
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            Debug.Log("XR Loader initialized: VR active");
            isVRActive = true;

            // Start XR
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
        else
        {
            Debug.Log("XR not found, falling back to desktop");
            isVRActive = false;
        }

        // 2. Enable/disable rigs based on the result
        xrOrigin.SetActive(isVRActive);
        desktopRig.SetActive(!isVRActive);
        desktop_view.SetActive(!isVRActive);
        if(isVRActive)
            hotspotManager.SetUIManager(vr_view_image_manager);
        else
        {
            hotspotManager.SetUIManager(desktop_view_image_manager);
            EnableDesktopMaps();
        }
        // Notify listeners that mode has been determined
        OnModeSelected?.Invoke(isVRActive);
    }

    void OnDestroy()
    {
        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }

    //void Start()
    //{
    //    //bool vrActive = XRSettings.isDeviceActive;
    //    //neste ponto ainda não ativa, apenas no update, ver se há uma forma melhor

    //    //xrOrigin.SetActive(vrActive);
    //    //desktopRig.SetActive(!vrActive);

    //    //Debug.Log("XR Active: " + XRSettings.isDeviceActive);
    //    //if (!XRSettings.isDeviceActive)
    //    //{
    //    //    EnableDesktopMaps();
    //    //}

    //}

    void Update()
    {
        //if (XRSettings.isDeviceActive)
        //{
        //    xrOrigin.SetActive(true);
        //    desktopRig.SetActive(false);
        //    desktop_view.SetActive(false);
        //    hotspotManager.SetUIManager(vr_view_image_manager);
        //}
        //else //desktop version
        //{
        //    xrOrigin.SetActive(false);
        //    desktopRig.SetActive(true);
        //    desktop_view.SetActive(true);
        //    EnableDesktopMaps();
        //    hotspotManager.SetUIManager(desktop_view_image_manager);
        //}
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
