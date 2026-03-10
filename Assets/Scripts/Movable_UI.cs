using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class Movable_UI : MonoBehaviour
{
    private bool isDragging = false;
    public InputActionAsset inputActions;
    private InputAction bButton;
    public Transform rightController;
    void Start()
    {
        bButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("BButton");
        bButton.Enable();
        isDragging = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            if (bButton.WasPressedThisFrame())
            {
                isDragging = false;
            }
            else
            {
                gameObject.transform.position = rightController.position + rightController.forward /* * 0.5f*/;
                gameObject.transform.rotation = rightController.rotation;
            }
        }
    }
}
