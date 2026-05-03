using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private RectTransform dragTarget;   // The panel to move (defaults to this)
    [SerializeField] private RectTransform dragArea;     // Optional: bounds (e.g., Canvas root)

    private RectTransform _target;
    private Canvas _canvas;
    private Camera _uiCamera;
    private Vector2 _pointerOffset;

    private void Awake()
    {
        _target = dragTarget ? dragTarget : (RectTransform)transform;

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("DraggableUI needs to be under a Canvas.");
            enabled = false;
            return;
        }

        // For Screen Space - Overlay, camera is null. Otherwise use the canvas world camera.
        _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Compute offset so the panel doesn't "snap" its pivot to the mouse.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _target, eventData.position, _uiCamera, out var localPointerPos))
        {
            _pointerOffset = localPointerPos;
        }

        // Optional: bring to front
        _target.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_target.parent, eventData.position, _uiCamera, out var parentLocalPoint))
            return;

        // Position so that the same point under the mouse stays under the mouse
        _target.anchoredPosition = parentLocalPoint - _pointerOffset;

        // Optional: clamp inside dragArea (like keeping it within the canvas)
        if (dragArea != null)
            ClampToArea(_target, dragArea);
    }

    private static void ClampToArea(RectTransform target, RectTransform area)
    {
        // Clamp target so its rect stays within area rect (both in area local space)
        var areaRect = area.rect;

        // target corners in world
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        // convert to area local
        for (int i = 0; i < 4; i++)
            corners[i] = area.InverseTransformPoint(corners[i]);

        float minX = corners[0].x;
        float maxX = corners[2].x;
        float minY = corners[0].y;
        float maxY = corners[2].y;

        Vector2 delta = Vector2.zero;

        if (minX < areaRect.xMin) delta.x += areaRect.xMin - minX;
        if (maxX > areaRect.xMax) delta.x -= maxX - areaRect.xMax;
        if (minY < areaRect.yMin) delta.y += areaRect.yMin - minY;
        if (maxY > areaRect.yMax) delta.y -= maxY - areaRect.yMax;

        target.anchoredPosition += delta;
    }
}