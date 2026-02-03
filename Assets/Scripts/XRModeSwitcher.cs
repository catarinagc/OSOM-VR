using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;


public class XRModeSwitcher : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    public InputActionReference moveAction;

    public GameObject xrOrigin;
    public GameObject desktopRig;

    void Start()
    {
        bool vrActive = XRSettings.isDeviceActive;

        xrOrigin.SetActive(vrActive);
        desktopRig.SetActive(!vrActive);

        Debug.Log("XR Active: " + XRSettings.isDeviceActive);
        if (!XRSettings.isDeviceActive)
        {
            EnableDesktopMaps();
        }
    }

    void Update()
    {
        Debug.Log(moveAction.action.ReadValue<Vector2>());
    }

    void EnableDesktopMaps()
    {
        // Find and enable the locomotion maps
        var leftLocomotion = inputActions.FindActionMap("XRI Left Locomotion");
        var rightLocomotion = inputActions.FindActionMap("XRI Right Locomotion");

        if (leftLocomotion != null) leftLocomotion.Enable();
        if (rightLocomotion != null) rightLocomotion.Enable();

        Debug.Log("Desktop input maps enabled");
    }
}
