using UnityEngine;
using TMPro;

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
}