using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
public class TooltipScript : MonoBehaviour
{
    public static TooltipScript Instance;
    public GameObject panel;
    public TMP_Text tooltipText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (panel.activeSelf)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            panel.transform.position = mousePos + new Vector2(25,-60);
        }
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
