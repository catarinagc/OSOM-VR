using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class HotspotClearImage : MonoBehaviour
{
    [SerializeField] Image_UI_Manager UIManager;
    [SerializeField] ImageAnnotationManager annotationManager;
    [SerializeField] RectTransform imageRect;
    [SerializeField] NoteMarker noteMarkerPrefab;
    [SerializeField] TMP_InputField noteInputField;
    public InspectionImage imageData;
    public int currentYear;
    public int currentHotspotId;
    public string currentDirection;
    public bool isFirstSlot;

    // public void OnClick()
    // {
    //     UIManager.HideItem(isFirstSlot);
    // }

    // public void OnConfirmNote()
    // {
    //     string message = noteInputField.text;

    //     if (string.IsNullOrWhiteSpace(message)) return;

    //     NoteData note = annotationManager.ConfirmNote(message);
    //     SpawnMarker(note);

    //     noteInputField.text = string.Empty;
    // }

    // public void SpawnMarker(NoteData note)
    // {
    //     NoteMarker marker = Instantiate(noteMarkerPrefab, imageRect);
    //     marker.Initialize(note, imageRect);
    // }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     // Ignore if it was a UI button or other element on top
    //     if (eventData.button != PointerEventData.InputButton.Left) return;

    //     Vector2 relativePos = GetRelativePosition(eventData.position);
    //     annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, relativePos, this);
    // }

    // private Vector2 GetRelativePosition(Vector2 screenClickPos)
    // {
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //         imageRect,
    //         screenClickPos,
    //         null, // pass Camera if using World Space canvas, null for Screen Space
    //         out Vector2 localPoint
    //     );

    //     Vector2 size = imageRect.rect.size;

    //     // Convert from local space (-size/2 to +size/2) to relative (0 to 1)
    //     return new Vector2(
    //         (localPoint.x / size.x) + 0.5f,
    //         (localPoint.y / size.y) + 0.5f
    //     );
    // }

    // public void SpawnMarkers()
    // {    
    //     ImageKey key = new ImageKey { year = currentYear, hotspotId = currentHotspotId, direction = currentDirection };
    //     List<NoteData> notes = annotationManager.GetNotesForImage(key);
    //     foreach (NoteData note in notes)
    //     {
    //         SpawnMarker(note);
    //     }
    // }

    // public void ClearNotes()
    // {
    //     foreach (Transform child in imageRect)
    //     {
    //         if (child.GetComponent<NoteMarker>() != null)
    //             Destroy(child.gameObject);
    //     }
    // }

    public void OnClickFullscreen()
    {
        UIManager.SetImageFullscreen(imageData);
    }
}
