using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class CloseUI : MonoBehaviour
{
    private InputAction yButton;
    [SerializeField] InputActionAsset inputActions;

    //void Start()
    //{
    //    yButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("YButton");
    //    yButton.Enable();
    //}
    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Update()
    //{
    //    if (Keyboard.current.escapeKey.wasPressedThisFrame || yButton.WasPressedThisFrame())
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}
}
