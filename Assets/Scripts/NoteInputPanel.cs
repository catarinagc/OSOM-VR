using UnityEngine;
using TMPro;
using System.Collections;
public class NoteInputPanel : MonoBehaviour
{
    [SerializeField] ImageAnnotationManager annotationManager;
    [SerializeField] TMP_InputField noteInputField;

    public void OnConfirmNote()
    {
        string message = noteInputField.text;
        if (string.IsNullOrWhiteSpace(message)) return;

        ImageDisplayController caller = annotationManager.GetPendingCaller();
        NoteData note = annotationManager.ConfirmNote(message);
        caller.SpawnMarker(note);

        noteInputField.text = string.Empty;
    }

    void OnEnable()
    {
        string existing = annotationManager.GetPendingMessage();
        noteInputField.text = existing ?? string.Empty;
        noteInputField.caretPosition = noteInputField.text.Length;
        noteInputField.stringPosition = noteInputField.text.Length;
        noteInputField.ForceLabelUpdate();
    }
}