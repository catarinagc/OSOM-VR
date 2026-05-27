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
    public bool isVR;
    private Vector2 lastClickRelativePos;

    public void SpawnMarker(NoteData note)
    {
        NoteMarker marker = Instantiate(noteMarkerPrefab, imageRect);
        marker.Initialize(note, imageRect);
        marker.displayController = this;
    }

    private float readyTime;
    private const float READY_DELAY = 0.3f; // seconds after panel opens

    void OnEnable()
    {
        readyTime = Time.time + READY_DELAY;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isVR)
        {
            if (Time.time < readyTime) return;
            lastClickRelativePos = GetRelativePositionVR(eventData);

            if (lastClickRelativePos.x < 0f || lastClickRelativePos.x > 1f ||
            lastClickRelativePos.y < 0f || lastClickRelativePos.y > 1f)
            return;

            annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, lastClickRelativePos, this);
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;
        Vector2 relativePos = GetRelativePosition(eventData.position);
        annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, relativePos, this);
    }

    // public void OnClickInteractVR()
    // {
    //     if (!isReady) //avoid first auto click when panel opens, dont know why this happens
    //     {
    //         isReady = true;
    //         return;
    //     }
    //     annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, lastClickRelativePos, this);
    // }

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
        Vector3 worldHitPos = eventData.pointerCurrentRaycast.worldPosition;

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

    public void EditNote(NoteData note, System.Action onConfirm = null)
    {
        annotationManager.EditNote(note, this, onConfirm);
    }

    public void DeleteNote(NoteData data)
    {
        annotationManager.DeleteNote(data);
    }
}
