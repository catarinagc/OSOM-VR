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
    public GameObject panelVR;
    public GameObject panelPC;
    public InspectionImage imageData;
    public int currentYear;
    public int currentHotspotId;
    public string currentDirection;
    private bool isReady = false;
    [SerializeField] bool canInteract;
    public bool isVR;
    private Vector2 lastClickRelativePos;

    public void SpawnMarker(NoteData note)
    {
        NoteMarker marker = Instantiate(noteMarkerPrefab, imageRect);
        //marker.transform.SetAsFirstSibling();
        marker.Initialize(note, imageRect, isVR, canInteract, panelVR, panelPC);
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
        if (!canInteract)
            return;
        
        if (isVR)
        {
            if (Time.time < readyTime) return;
            lastClickRelativePos = GetRelativePositionVR(eventData);

            if (lastClickRelativePos.x < 0f || lastClickRelativePos.x > 1f || lastClickRelativePos.y < 0f || lastClickRelativePos.y > 1f)
                return;

            annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, lastClickRelativePos, this);
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;
        Vector2 relativePos = GetRelativePosition(eventData.position);

        Debug.Log("REL " + relativePos);

        if (relativePos.x < 0f || relativePos.x > 1f || relativePos.y < 0f || relativePos.y > 1f)
            return;
        
        annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, relativePos, this);
    }

    private Vector2 GetRelativePosition(Vector2 screenClickPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageRect,
            screenClickPos,
            null,
            out Vector2 localPoint
        );

        Vector2 size = GetActualImageSize();

        return new Vector2(
            (localPoint.x / size.x) + 0.5f,
            (localPoint.y / size.y) + 0.5f
        );
    }

    private Vector2 GetActualImageSize()
    {
        Image img = imageRect.GetComponent<Image>();
        if (img == null || img.sprite == null) return imageRect.rect.size;

        float spriteWidth = img.sprite.rect.width;
        float spriteHeight = img.sprite.rect.height;
        float spriteAspect = spriteWidth / spriteHeight;

        float rectWidth = imageRect.rect.width;
        float rectHeight = imageRect.rect.height;
        float rectAspect = rectWidth / rectHeight;

        if (spriteAspect > rectAspect)
        {
            // Letterboxed top and bottom
            return new Vector2(rectWidth, rectWidth / spriteAspect);
        }
        else
        {
            // Letterboxed left and right
            return new Vector2(rectHeight * spriteAspect, rectHeight);
        }
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
