using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ImageDisplayController : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler
{
    [SerializeField] public Image_UI_Manager UIManager;
    [SerializeField] public ImageAnnotationManager annotationManager;
    [SerializeField] RectTransform imageRect;
    [SerializeField] NoteMarker noteMarkerPrefab;
    [SerializeField] public TMP_InputField noteInputField;
    public SyncZoomVR_Manager syncManager;
    public GameObject panelVR;
    public GameObject panelPC;
    public InspectionImage imageData;
    public int currentYear;
    public int currentHotspotId;
    public string currentDirection;
    private bool isReady = false;
    private bool settingFromDouble = false;
    [SerializeField] bool canInteract;
    public bool isVR;
    private Vector2 lastClickRelativePos;
    private bool isSettingMarker = false;
    private NoteMarker ghostMarker;
    [SerializeField] public Transform vrControllerRay;

    //
    [SerializeField] public InputActionReference leftGripAction;
    [SerializeField] public InputActionReference rightGripAction;
    [SerializeField] public Transform leftControllerTransform;
    [SerializeField] public Transform rightControllerTransform;
    [SerializeField] public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor leftInteractor;
    [SerializeField] public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor rightInteractor;
    [SerializeField] private VRZoomImage vrZoom;

    public void SpawnMarker(NoteData note)
    {
        NoteMarker marker = Instantiate(noteMarkerPrefab, imageRect);
        marker.Initialize(note, imageRect, isVR, canInteract, panelVR, panelPC);
        marker.displayController = this;
        marker.uI_Manager = UIManager.UI_Manager; //TODO not great melhorar depois
    }

    private float readyTime;
    private const float READY_DELAY = 0.3f;

    void OnEnable()
    {
        readyTime = Time.time + READY_DELAY;
    }

    void Start()
    {
        isSettingMarker= false;
        if (isVR)
        {
            vrZoom.leftGripAction = leftGripAction;
            vrZoom.rightGripAction = rightGripAction;
            vrZoom.leftControllerTransform = leftControllerTransform;
            vrZoom.rightControllerTransform = rightControllerTransform;
            vrZoom.leftInteractor = leftInteractor;
            vrZoom.rightInteractor = rightInteractor;
            vrZoom.syncManager = syncManager;
        }
    }

    public void OnCLick()
    {
        if (!canInteract) return;

        if (isSettingMarker)
        {
            CancelPlacement();
            return;
        }

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

        if(isVR) return;

        // PC path unchanged
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 relativePos2 = GetRelativePosition(mousePos);

        relativePos2.x = Mathf.Clamp01(relativePos2.x);
        relativePos2.y = Mathf.Clamp01(relativePos2.y);

        PositionMarkerAt(ghostMarker, relativePos2);

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

    private Vector2 lastVRRelativePos;

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!isSettingMarker || ghostMarker == null || !isVR) return;
        if (vrControllerRay == null) return;

        Ray ray = new Ray(vrControllerRay.position, vrControllerRay.forward);
        Plane imagePlane = new Plane(-imageRect.forward, imageRect.position);

        if (imagePlane.Raycast(ray, out float distance))
        {
            Vector3 worldHitPos = ray.GetPoint(distance);
            Vector2 relativePos = GetRelativePositionVR(worldHitPos);
            relativePos.x = Mathf.Clamp01(relativePos.x);
            relativePos.y = Mathf.Clamp01(relativePos.y);
            PositionMarkerAt(ghostMarker, relativePos);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canInteract) return;
        if (!isVR && isSettingMarker) return;

        if (isVR)
        {
            if (Time.time < readyTime) return;
            if (!isSettingMarker) return;
            if (vrControllerRay == null) return;

            Ray ray = new Ray(vrControllerRay.position, vrControllerRay.forward);
            Plane imagePlane = new Plane(-imageRect.forward, imageRect.position);

            if (!imagePlane.Raycast(ray, out float distance)) return;

            Vector3 worldHitPos = ray.GetPoint(distance);
            Vector2 relativePos = GetRelativePositionVR(worldHitPos);
            relativePos.x = Mathf.Clamp01(relativePos.x);
            relativePos.y = Mathf.Clamp01(relativePos.y);
            PlaceMarkerAndOpenInput(relativePos);
        }
    }

    // Overload that takes worldHitPos directly instead of eventData
    private Vector2 GetRelativePositionVR(Vector3 worldHitPos)
    {
        Vector2 localPoint = imageRect.InverseTransformPoint(worldHitPos);
        Vector2 size = GetActualImageSize();
        return new Vector2(
            (localPoint.x / size.x) + 0.5f,
            (localPoint.y / size.y) + 0.5f
        );
    }

    private Vector2 WorldHitToRelative(Vector3 worldHitPos)
    {
        Vector2 localPoint = imageRect.InverseTransformPoint(worldHitPos);
        Vector2 actualSize = GetActualImageSize();

        // localPoint is in rect space (centered at 0,0)
        // clamp to actual image bounds before normalizing
        localPoint.x = Mathf.Clamp(localPoint.x, -actualSize.x * 0.5f, actualSize.x * 0.5f);
        localPoint.y = Mathf.Clamp(localPoint.y, -actualSize.y * 0.5f, actualSize.y * 0.5f);

        return new Vector2(
            (localPoint.x / actualSize.x) + 0.5f,
            (localPoint.y / actualSize.y) + 0.5f
        );
    }

    public void SetMrakerOnDoubleImage()
    {
        isSettingMarker = true;
        settingFromDouble = true;
    }

    private void PlaceMarkerAndOpenInput(Vector2 relativePos)
    {
        // Hide ghost, exit placement mode
        isSettingMarker = false;
        settingFromDouble = false;
        if (ghostMarker != null)
            ghostMarker.gameObject.SetActive(false);

        annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, relativePos, this);
    }

    public Vector2 GetRelativePosition(Vector2 screenClickPos)
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
        if (img == null || img.sprite == null || !img.preserveAspect)
            return imageRect.rect.size;

        float spriteWidth = img.sprite.rect.width;
        float spriteHeight = img.sprite.rect.height;
        float spriteAspect = spriteWidth / spriteHeight;

        float rectWidth = imageRect.rect.width;
        float rectHeight = imageRect.rect.height;
        float rectAspect = rectWidth / rectHeight;

        if (spriteAspect > rectAspect)
            return new Vector2(rectWidth, rectWidth / spriteAspect);
        else
            return new Vector2(rectHeight * spriteAspect, rectHeight);
    }

    public void SpawnMarkers()
    {    
        ImageKey key = new ImageKey { year = currentYear, hotspotId = currentHotspotId, direction = currentDirection };
        List<NoteData> notes = annotationManager.GetNotesForImage(key);
        Debug.Log($"SpawnMarkers: spawning {notes.Count} markers");
        foreach (NoteData note in notes)
        {
            SpawnMarker(note);
        }
    }

    public void ClearNotes()
    {
        var markers = new System.Collections.Generic.List<GameObject>();
        foreach (Transform child in imageRect)
        {
            if (child.GetComponent<NoteMarker>() != null)
                markers.Add(child.gameObject);
        }
        Debug.Log($"ClearNotes: found {markers.Count} markers to destroy, imageRect childCount: {imageRect.childCount}");
        foreach (var m in markers)
            DestroyImmediate(m);
        Debug.Log($"ClearNotes: after destroy, imageRect childCount: {imageRect.childCount}");
    }

    public void EditNote(NoteData note, System.Action onConfirm = null)
    {
        annotationManager.EditNote(note, this, onConfirm);
    }

    public void DeleteNote(NoteData data)
    {
        annotationManager.DeleteNote(data);
    }

    // Expose relative position calculation for the coordinator
    public Vector2 GetRelativePositionFromScreen(Vector2 screenPos)
    {
        return GetRelativePosition(screenPos);
    }

    // Called by coordinator instead of OnCLick — skips the ghost/Update loop entirely
    public void PlaceNoteAtPosition(Vector2 relativePos)
    {
        if (!canInteract) return;
        annotationManager.OpenNoteInput(currentYear, currentHotspotId, currentDirection, relativePos, this);
    }
}
