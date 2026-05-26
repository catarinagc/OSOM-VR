using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // or UnityEngine.UI if using standard Text

public class NoteMarker : MonoBehaviour, IPointerClickHandler
{
    public NoteData data;
    [SerializeField] GameObject tooltipPanel;     // popup that shows the message
    [SerializeField] TMP_Text tooltipText;        // text inside the popup

    public void Initialize(NoteData noteData, RectTransform imageRect)
    {
        data = noteData;
        SetPosition(noteData.relativePos, imageRect);
    }

    private void SetPosition(Vector2 relativePos, RectTransform imageRect)
    {
        Vector2 size = imageRect.rect.size;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(
            (relativePos.x - 0.5f) * size.x,
            (relativePos.y - 0.5f) * size.y
        );
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (eventData.button != PointerEventData.InputButton.Left) return;
        
        tooltipText.text = $"{data.message}\n<size=10>{data.created}</size>";
        tooltipPanel.SetActive(!tooltipPanel.activeSelf); // toggle on/off
    }
}