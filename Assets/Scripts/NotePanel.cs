using UnityEngine;
using TMPro;
public class NotePanel : MonoBehaviour
{
    [SerializeField] TMP_Text noteText;
    [SerializeField] TMP_Text noteData;
    private NoteMarker marker;
    
    public void Open(string message, string date, NoteMarker marker)
    {
        noteText.text = message;
        noteData.text = date;
        this.marker = marker;
    }

    public void EditMessage()
    {
        marker.EditMessage();
    }

    public void DeleteNote()
    {
        marker.DeleteNote();
        gameObject.SetActive(false);
    }

    public void DisablePanel()
    {
        gameObject.SetActive(false);
    }
}
