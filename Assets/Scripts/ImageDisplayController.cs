using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
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
    private bool isSettingMarker = false;
    private NoteMarker ghostMarker;

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

    void OnStart()
    {
        isSettingMarker= false;
    }

    public void OnCLick()
    {
        if (!canInteract) return;

        isSettingMarker = true;

        if (ghostMarker == null)
        {
            // Create a temporary NoteData so Initialize doesn't blow up
            NoteData placeholder = new NoteData();
            ghostMarker = Instantiate(noteMarkerPrefab, imageRect);
            ghostMarker.Initialize(placeholder, imageRect, isVR, canInteract: false, panelVR, panelPC);
            // Make the ghost invisible to raycasts so clicks pass through to the image
            CanvasGroup cg = ghostMarker.gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
        }

        ghostMarker.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isSettingMarker || ghostMarker == null) return;

        if (isVR) return;

        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 relativePos = GetRelativePosition(mousePos);

        relativePos.x = Mathf.Clamp01(relativePos.x);
        relativePos.y = Mathf.Clamp01(relativePos.y);

        PositionMarkerAt(ghostMarker, relativePos);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 clickRelativePos = GetRelativePosition(mousePos);

            if (clickRelativePos.x >= 0f && clickRelativePos.x <= 1f &&
                clickRelativePos.y >= 0f && clickRelativePos.y <= 1f)
            {
                PlaceMarkerAndOpenInput(clickRelativePos);
            }
        }
    }

    private void PositionMarkerAt(NoteMarker marker, Vector2 relativePos)
    {
        Vector2 size = GetActualImageSize();
        float localX = (relativePos.x - 0.5f) * size.x;
        float localY = (relativePos.y - 0.5f) * size.y;
        marker.GetComponent<RectTransform>().anchoredPosition = new Vector2(localX, localY);
    }

    public void CancelPlacement()
    {
        isSettingMarker = false;
        if (ghostMarker != null)
            ghostMarker.gameObject.SetActive(false);
    }

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (!canInteract || !isSettingMarker)
    //         return;
        
    //     if (isVR)
    //     {
    //         if (Time.time < readyTime) return;
    //         lastClickRelativePos = GetRelativePositionVR(eventData);

    //         if (lastClickRelativePos.x < 0f || lastClickRelativePos.x > 1f || lastClickRelativePos.y < 0f || lastClickRelativePos.y > 1f)
    //             return;

    //         annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, lastClickRelativePos, this);
    //         return;
    //     }

    //     if (eventData.button != PointerEventData.InputButton.Right) return;
    //     Vector2 relativePos = GetRelativePosition(eventData.position);

    //     Debug.Log("REL " + relativePos);

    //     if (relativePos.x < 0f || relativePos.x > 1f || relativePos.y < 0f || relativePos.y > 1f)
    //         return;
        
    //     annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, relativePos, this);
    // }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canInteract) return;
        
        // Placement is now handled in Update() for PC
        if (!isVR && isSettingMarker) return;

        if (isVR)
        {
            if (Time.time < readyTime) return;

            lastClickRelativePos = GetRelativePositionVR(eventData);
            if (lastClickRelativePos.x < 0f || lastClickRelativePos.x > 1f ||
                lastClickRelativePos.y < 0f || lastClickRelativePos.y > 1f) return;

            PlaceMarkerAndOpenInput(lastClickRelativePos);
            return;
        }
    }

    private void PlaceMarkerAndOpenInput(Vector2 relativePos)
    {
        // Hide ghost, exit placement mode
        isSettingMarker = false;
        if (ghostMarker != null)
            ghostMarker.gameObject.SetActive(false);

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
