using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class VRController : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    public DynamicMoveProvider moveProvider;
    public enum MovementMode { Walking, Flying }
    public MovementMode currentMode;
    public Transform initWalkPos;
    private InputAction aButton;
    
    void Awake()
    {
        aButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("AButton");
        aButton.Enable();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveProvider.enableFly = true;
        currentMode = MovementMode.Flying;
    }

    // Update is called once per frame
    void Update()
    {
        if (aButton.WasPressedThisFrame())
        {
            toggleFly();
        }
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
}
