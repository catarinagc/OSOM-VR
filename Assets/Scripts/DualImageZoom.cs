using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DualImageZoom : MonoBehaviour,
    IScrollHandler, IBeginDragHandler, IDragHandler
{
    [Header("The images themselves (children of the masked wrappers)")]
    [SerializeField] private RectTransform leftImage;
    [SerializeField] private RectTransform rightImage;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 10f;

    public Image left;
    public Image right;

    private float _zoom = 1f;
    private Vector2 _pan = Vector2.zero;
    private Vector2 _lastDragPos;
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        
        SetupWrapperAnchors(leftImage.parent as RectTransform,  new Vector2(0f,   0f), new Vector2(0.5f, 1f));
        SetupWrapperAnchors(rightImage.parent as RectTransform, new Vector2(0.5f, 0f), new Vector2(1f,   1f));

        SetupImageAnchors(leftImage);
        SetupImageAnchors(rightImage);

        Apply();
    }

    private static void SetupWrapperAnchors(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax)
    {
        rt.anchorMin  = anchorMin;
        rt.anchorMax  = anchorMax;
        rt.pivot      = new Vector2(0.5f, 0.5f);
        rt.offsetMin  = Vector2.zero;
        rt.offsetMax  = Vector2.zero;
    }

    private static void SetupImageAnchors(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;         // stretch both axes
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;         // left/bottom = 0
        rt.offsetMax = Vector2.zero;         // right/top = 0
    }

    public void OnScroll(PointerEventData eventData)
    {
        _zoom = Mathf.Clamp(_zoom + eventData.scrollDelta.y * zoomSpeed, minZoom, maxZoom);
        ClampPan();
        Apply();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - _lastDragPos;
        _lastDragPos = eventData.position;

        _pan += delta;
        ClampPan();
        Apply();
    }

    private void Apply()
    {
        Vector3 scale = Vector3.one * _zoom;

        leftImage.localScale        = scale;
        leftImage.anchoredPosition  = _pan;

        rightImage.localScale       = scale;
        rightImage.anchoredPosition = _pan;
    }

    private void ClampPan()
    {
        RectTransform wrapper = leftImage.parent as RectTransform;
        Vector2 wrapperSize   = wrapper.rect.size;

        float extraX = Mathf.Max(0f, (wrapperSize.x * _zoom - wrapperSize.x) * 0.5f);
        float extraY = Mathf.Max(0f, (wrapperSize.y * _zoom - wrapperSize.y) * 0.5f);

        _pan.x = Mathf.Clamp(_pan.x, -extraX, extraX);
        _pan.y = Mathf.Clamp(_pan.y, -extraY, extraY);
    }

    public void OnCloseImage()
    {
        _zoom = 1f;
        _pan  = Vector2.zero;
        Apply();
    }
}
