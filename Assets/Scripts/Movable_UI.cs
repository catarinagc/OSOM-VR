using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
public class Movable_UI : MonoBehaviour
{
    private bool isDragging = false;
    public InputActionAsset inputActions;
    private InputAction bButton;
    public Transform rightController;
    public Transform leftController;
    private float startDistance;
    private Vector3 startScale;
    private InputAction leftTrigger;
    private InputAction rightTrigger;
    private bool resizing = false;
    public Button moveButton;

    void Start()
    {
        bButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("BButton");
        bButton.Enable();
        isDragging = true;
        leftTrigger = inputActions.FindActionMap("XRI Left Interaction").FindAction("trigger");
        leftTrigger.Enable();
        rightTrigger = inputActions.FindActionMap("XRI Right Interaction").FindAction("trigger");
        rightTrigger.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        bool leftHover = moveButton.GetComponent<RectTransform>().rect.Contains(
            moveButton.transform.InverseTransformPoint(leftController.position));
        bool rightHover = moveButton.GetComponent<RectTransform>().rect.Contains(
            moveButton.transform.InverseTransformPoint(rightController.position));

        // Check triggers
        bool leftPressed = leftTrigger.ReadValue<float>() > 0.1f;
        bool rightPressed = rightTrigger.ReadValue<float>() > 0.1f;

        if (leftHover && rightHover && leftPressed && rightPressed)
        {
            if (!resizing)
            {
                // start resizing
                resizing = true;
                startDistance = Vector3.Distance(leftController.position, rightController.position);
                startScale = transform.localScale;
            }

            float currentDistance = Vector3.Distance(leftController.position, rightController.position);
            float scaleFactor = currentDistance / startDistance;

            transform.localScale = startScale * scaleFactor;
        }
        else
        {
            resizing = false;
        }

    }

    // public void OnClick()
    // {
    //     isDragging = true;
    // }
}
