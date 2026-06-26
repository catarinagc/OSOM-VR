using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.UI;
public class VRZoomImage : MonoBehaviour
{
    [Header("The image (child of the masked wrapper)")]
    [SerializeField] private RectTransform image;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 10f;

    [Header("Pan")]
    [SerializeField] private float panSpeed = 300f;

    [Header("Input Actions")]
    [SerializeField] public InputActionReference leftGripAction;
    [SerializeField] public InputActionReference rightGripAction;

    [Header("Controller Transforms")]
    [SerializeField] public Transform leftControllerTransform;
    [SerializeField] public Transform rightControllerTransform;
    
    [Header("Interactors")]
    [SerializeField] public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor leftInteractor;
    [SerializeField] public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor rightInteractor;

    [Header("Interactable")]
    [SerializeField] private XRGrabInteractable grabInteractable;

    [SerializeField] private Button pinButton;

    public SyncZoomVR_Manager syncManager;

    private float _zoom = 1f;
    private Vector2 _pan = Vector2.zero;

    // Pinch state
    private bool _wasPinchingBoth = false;
    private bool _wasPinchingSingle = false;
    private float _lastPinchDistance = 0f;
    private Vector3 _lastMidpoint;
    private bool isPinned = false;

    private ColorBlock defaultPinButtonColors;
    private ColorBlock selectedPinButtonColors;

    void Start()
    {
        defaultPinButtonColors = pinButton.colors;
        selectedPinButtonColors = pinButton.colors;
        selectedPinButtonColors.normalColor = selectedPinButtonColors.pressedColor;
        selectedPinButtonColors.highlightedColor = selectedPinButtonColors.pressedColor;
    }

    public void OnClickPin()
    {
        TelemetryLogger.Instance.LogUIInteraction("Pin Image");
        isPinned = !isPinned;
        if (isPinned)
        {
            syncManager.AddImage(this);
            pinButton.colors = selectedPinButtonColors;
        }
        else
        {
            syncManager.RemoveImage(this);
            pinButton.colors = defaultPinButtonColors;
                        
        }
    }

    private void Update()
    {
        if (IsAnyControllerPointingAtImage())
        {
            HandlePinchZoomAndPan();
        }
        else
        {
            _wasPinchingBoth   = false;
            _wasPinchingSingle = false;
        }

        if (isPinned)
            syncManager.UpdateAllImages(_zoom, _pan);
        else
            Apply(_zoom, _pan);
    }

    private bool IsAnyControllerPointingAtImage()
    {
        return grabInteractable.isHovered;
    }
    
    private void HandlePinchZoomAndPan()
    {
        float leftGrip  = leftGripAction.action.ReadValue<float>();
        float rightGrip = rightGripAction.action.ReadValue<float>();

        bool isPinching = leftGrip > 0.5f && rightGrip > 0.5f;
        bool singleGrip = (leftGrip > 0.5f) ^ (rightGrip > 0.5f);

        if (isPinching)
        {
            float currentDistance = Vector3.Distance(
                leftControllerTransform.position,
                rightControllerTransform.position
            );

            Vector3 currentMidpoint = (leftControllerTransform.position
                                    + rightControllerTransform.position) / 2f;

            if (_wasPinchingBoth)
            {
                float zoomDelta = (currentDistance - _lastPinchDistance) * zoomSpeed;
                _zoom = Mathf.Clamp(_zoom + zoomDelta, minZoom, maxZoom);
                ClampPan();
            }

            _lastPinchDistance = currentDistance;
            _lastMidpoint = currentMidpoint;
            _wasPinchingBoth = true;
            _wasPinchingSingle = false; // reset so single grip starts fresh
        }
        else if (singleGrip)
        {
            Transform activeController = (leftGrip > 0.5f)
                ? leftControllerTransform
                : rightControllerTransform;

            if (_wasPinchingSingle)
            {
                // Delta from last known controller position — never jumps
                Vector3 delta = activeController.position - _lastMidpoint;
                _pan += new Vector2(
                    Vector3.Dot(delta, transform.right),
                    Vector3.Dot(delta, transform.up)
                ) * panSpeed;

                ClampPan();
            }

            // Always update to current position AFTER calculating delta
            _lastMidpoint      = activeController.position;
            _wasPinchingSingle = true;
            _wasPinchingBoth   = false; // reset so both-grip starts fresh
        }
        else
        {
            _wasPinchingBoth   = false;
            _wasPinchingSingle = false;
        }
    }

    public void Apply(float zoom, Vector2 pan)
    {
        // image.localScale = Vector3.one * _zoom;
        // image.anchoredPosition = _pan;
        image.localScale = Vector3.one * zoom;
        image.anchoredPosition = pan;
        _zoom = zoom;
        _pan  = pan;
    }

    private void ClampPan()
    {
        RectTransform wrapper = image.parent as RectTransform;
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
        Apply(_zoom, _pan);
        syncManager.RemoveImage(this);
    }

    public float getZoom() {
        return _zoom;
    }

    public Vector2 getPan()
    {
        return _pan;
    }
}