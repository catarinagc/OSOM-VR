using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string tooltipText;
    public bool isActive = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isActive)
            TooltipScript.Instance.Show(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipScript.Instance.Hide();
    }

    private void OnDisable()
    {
        // Hide tooltip if this object gets disabled while hovered
        if (TooltipScript.Instance != null)
        {
            TooltipScript.Instance.Hide();
        }
    }
}
