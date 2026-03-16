using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string tooltipText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipScript.Instance.Show(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipScript.Instance.Hide();
    }
}
