using UnityEngine;
using UnityEngine.InputSystem;

public class VR_Controller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public InputActionReference moveAction; // Left Hand Joystick
    public float speed = 5f;
    private Transform camTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        
        Vector3 direction = (camTransform.forward * input.y) + (camTransform.right * input.x);
        
        transform.position += direction * speed * Time.deltaTime;
    }
}
