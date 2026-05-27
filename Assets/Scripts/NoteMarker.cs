using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // or UnityEngine.UI if using standard Text

public class NoteMarker : MonoBehaviour
{
    public NoteData data;
    [SerializeField] GameObject tooltipPanel;
    [SerializeField] TMP_Text tooltipText;
    [SerializeField] TMP_Text tooltipTextDate;
    public ImageDisplayController displayController;

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

    public void OnClick()
    {
        tooltipText.text = data.message;
        tooltipTextDate.text = data.created;
        tooltipPanel.SetActive(true);
    }

    public void EditMessage()
    {
        displayController.annotationManager.EditNote(data, displayController, () =>
        {
            tooltipPanel.SetActive(false);
        });
    }

    public void DeleteNote()
    {
        displayController.DeleteNote(data);
        Destroy(gameObject);
    }

    public void DisablePanel()
    {
        tooltipPanel.SetActive(false);
    }
}