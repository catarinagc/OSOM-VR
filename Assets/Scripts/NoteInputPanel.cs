using UnityEngine;
using TMPro;
using System.Collections;
public class NoteInputPanel : MonoBehaviour
{
    [SerializeField] ImageAnnotationManager annotationManager;
    [SerializeField] TMP_InputField noteInputField;
    [SerializeField] GameObject keyboard;

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
        keyboard.SetActive(true);
    }

    public void CancelNote()
    {
        noteInputField.text = "";
        //keyboard.SetActive(false);
        gameObject.SetActive(false);
    }
}