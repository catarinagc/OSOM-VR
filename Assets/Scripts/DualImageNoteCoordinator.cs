using UnityEngine;
using UnityEngine.InputSystem;

public class DualImageNoteCoordinator : MonoBehaviour
{
    [SerializeField] private ImageDisplayController leftController;
    [SerializeField] private ImageDisplayController rightController;
    [SerializeField] private RectTransform leftWrapper;
    [SerializeField] private RectTransform rightWrapper;
    [SerializeField] private NoteMarker noteMarkerPrefab;

    private bool _waitingForPlacement = false;
    private ImageDisplayController _hoveredController = null;
    private RectTransform _hoveredWrapper = null;
    private NoteMarker _ghostMarker;

    public void OnAddNoteClick()
    {
        _waitingForPlacement = true;
        EnsureGhostExists();
    }

    private void EnsureGhostExists()
    {
        if (_ghostMarker != null) return;

        NoteData placeholder = new NoteData();
        _ghostMarker = Instantiate(noteMarkerPrefab, leftWrapper);
        _ghostMarker.Initialize(placeholder, leftWrapper, false, false, null, null);
        CanvasGroup cg = _ghostMarker.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        _ghostMarker.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!_waitingForPlacement) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Detect which side mouse is over
        if (IsMouseOverRect(leftWrapper, mousePos))
        {
            _hoveredWrapper    = leftWrapper;
            _hoveredController = leftController;
        }
        else if (IsMouseOverRect(rightWrapper, mousePos))
        {
            _hoveredWrapper    = rightWrapper;
            _hoveredController = rightController;
        }
        else
        {
            _hoveredWrapper    = null;
            _hoveredController = null;
        }

        // Move and show/hide ghost
        if (_hoveredWrapper != null)
        {
            if (_ghostMarker.transform.parent != _hoveredWrapper)
                _ghostMarker.transform.SetParent(_hoveredWrapper, false);

            _ghostMarker.gameObject.SetActive(true);
            MoveGhostToMouse(mousePos, _hoveredWrapper);
        }
        else
        {
            _ghostMarker.gameObject.SetActive(false);
        }

        // On click, convert mouse pos to relative image coords and place directly
        if (Mouse.current.leftButton.wasPressedThisFrame && _hoveredController != null)
        {
            Vector2 relativePos = GetRelativePosition(mousePos, _hoveredController);

            if (relativePos.x >= 0f && relativePos.x <= 1f &&
                relativePos.y >= 0f && relativePos.y <= 1f)
            {
                _ghostMarker.gameObject.SetActive(false);
                _hoveredController.PlaceNoteAtPosition(relativePos);
            }

            _waitingForPlacement = false;
            _hoveredController   = null;
            _hoveredWrapper      = null;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cancel();
        }
    }

    private void MoveGhostToMouse(Vector2 screenPos, RectTransform wrapper)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wrapper, screenPos, null, out Vector2 localPoint
        );
        _ghostMarker.GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    private Vector2 GetRelativePosition(Vector2 screenPos, ImageDisplayController controller)
    {
        return controller.GetRelativePositionFromScreen(screenPos);
    }

    private void Cancel()
    {
        _waitingForPlacement = false;
        _hoveredController   = null;
        _hoveredWrapper      = null;
        if (_ghostMarker != null)
            _ghostMarker.gameObject.SetActive(false);
    }

    private bool IsMouseOverRect(RectTransform rt, Vector2 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos, null);
    }
}