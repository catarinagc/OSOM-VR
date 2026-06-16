using UnityEngine;
using TMPro;
using System.Collections;
public class NoteInputPanel : MonoBehaviour
{
    [SerializeField] ImageAnnotationManager annotationManager;
    [SerializeField] TMP_InputField noteInputField;

    // public void OnConfirmNote()
    // {
    //     string message = noteInputField.text;
    //     if (string.IsNullOrWhiteSpace(message)) return;

    //     ImageDisplayController caller = annotationManager.GetPendingCaller();
    //     NoteData note = annotationManager.ConfirmNote(message);
    //     //caller.SpawnMarker(note);

    //     noteInputField.text = "";
    // }

    public void OnConfirmNote()
    {
        string message = noteInputField.text;
        if (string.IsNullOrWhiteSpace(message)) return;

        ImageDisplayController caller = annotationManager.GetPendingCaller();
        bool isEditing = annotationManager.IsEditing();
        NoteData note = annotationManager.ConfirmNote(message);
        
        if (!isEditing)
            caller.SpawnMarker(note); // only spawn for new notes

        noteInputField.text = "";
    }

    void OnEnable()
    {
        string existing = annotationManager.GetPendingMessage();
        noteInputField.text = existing ?? "";
        noteInputField.caretPosition = noteInputField.text.Length;
        noteInputField.stringPosition = noteInputField.text.Length;
        noteInputField.ForceLabelUpdate();
    }

    public void CancelNote()
    {
        noteInputField.text = "";
        gameObject.SetActive(false);
    }
}