using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TooltipScript : MonoBehaviour
{
    public static TooltipScript Instance;
    public GameObject panel;
    public TMP_Text tooltipText;
    public Vector2 offset = new Vector2(25, 40);

    private RectTransform panelRect;
    private Vector3[] corners = new Vector3[4];

    void Awake()
    {
        Instance = this;
        panelRect = panel.GetComponent<RectTransform>();
        Hide();
    }

    void Update()
    {
        if (panel.activeSelf)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            panelRect.position = mousePos + offset;
            Vector3 correction = ClampToScreen();
            if (correction != Vector3.zero)
            {
                Vector2 negOffset = new Vector2(offset.x, -offset.y);
                panelRect.position = mousePos + negOffset;
                
            }
            correction = ClampToScreen();
            if (correction != Vector3.zero)
                panelRect.position += correction;
        }
    }

    Vector3 ClampToScreen()
    {
        panelRect.GetWorldCorners(corners);
        // corners[0] = bottom-left, corners[2] = top-right

        Vector3 correction = Vector3.zero;

        float overflowRight = corners[2].x - Screen.width;
        if (overflowRight > 0) correction.x -= overflowRight;

        float overflowLeft = -corners[0].x;
        if (overflowLeft > 0) correction.x += overflowLeft;

        float overflowTop = corners[2].y - Screen.height;
        if (overflowTop > 0) correction.y -= overflowTop;

        float overflowBottom = -corners[0].y;
        if (overflowBottom > 0) correction.y += overflowBottom;


        return correction;
        //panelRect.position += correction;
    }

    public void Show(string text)
    {
        tooltipText.text = text;
        panel.SetActive(true);
        // Force layout to update immediately so size is correct before clamping
        Canvas.ForceUpdateCanvases();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}