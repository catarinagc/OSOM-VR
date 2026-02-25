using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class CloseUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            gameObject.SetActive(false);
        }
    }
}
