using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ImageDisplayController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public Image_UI_Manager UIManager;
    [SerializeField] public ImageAnnotationManager annotationManager;
    [SerializeField] RectTransform imageRect;
    [SerializeField] NoteMarker noteMarkerPrefab;
    [SerializeField] public TMP_InputField noteInputField;
    public InspectionImage imageData;
    public int currentYear;
    public int currentHotspotId;
    public string currentDirection;
    private bool isReady = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // public void OnConfirmNote()
    // {
    //     Debug.Log("CONFIRM");
    //     string message = noteInputField.text;

    //     if (string.IsNullOrWhiteSpace(message)) return;

    //     NoteData note = annotationManager.ConfirmNote(message);
    //     SpawnMarker(note);

    //     noteInputField.text = string.Empty;
    // }

    public void SpawnMarker(NoteData note)
    {
        NoteMarker marker = Instantiate(noteMarkerPrefab, imageRect);
        marker.Initialize(note, imageRect);
    }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     // Ignore if it was a UI button or other element on top
    //     if (eventData.button != PointerEventData.InputButton.Left) return;

    //     //Vector2 relativePos = GetRelativePosition(eventData.position);
    //     // Vector2 relativePos = GetRelativePositionVR(eventData);
    //     // annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, relativePos, this);
    // }

    private Vector2 lastClickRelativePos;

    public void OnPointerClick(PointerEventData eventData)
    {
        lastClickRelativePos = GetRelativePositionVR(eventData);
    }

    public void OnClickInteractVR()
    {
        if (!isReady) //avoid first auto click when panel opens, dont know why this happens
        {
            isReady = true;
            return;
        }
        annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, lastClickRelativePos, this);
    }

    private Vector2 GetRelativePosition(Vector2 screenClickPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageRect,
            screenClickPos,
            null, // pass Camera if using World Space canvas, null for Screen Space
            out Vector2 localPoint
        );

        Vector2 size = imageRect.rect.size;

        // Convert from local space (-size/2 to +size/2) to relative (0 to 1)
        return new Vector2(
            (localPoint.x / size.x) + 0.5f,
            (localPoint.y / size.y) + 0.5f
        );
    }

    private Vector2 GetRelativePositionVR(PointerEventData eventData)
    {
        // In VR the worldPosition is where the ray hit the UI element
        Vector3 worldHitPos = eventData.pointerCurrentRaycast.worldPosition;

        // Convert world position to local position on the imageRect
        Vector2 localPoint = imageRect.InverseTransformPoint(worldHitPos);

        Vector2 size = imageRect.rect.size;

        return new Vector2(
            (localPoint.x / size.x) + 0.5f,
            (localPoint.y / size.y) + 0.5f
        );
    }

    public void SpawnMarkers()
    {    
        ImageKey key = new ImageKey { year = currentYear, hotspotId = currentHotspotId, direction = currentDirection };
        List<NoteData> notes = annotationManager.GetNotesForImage(key);
        foreach (NoteData note in notes)
        {
            SpawnMarker(note);
        }
    }

    public void ClearNotes()
    {
        foreach (Transform child in imageRect)
        {
            if (child.GetComponent<NoteMarker>() != null)
                Destroy(child.gameObject);
        }
    }
}
