using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
public class SnapMenuToPlayer : MonoBehaviour
{
    public Transform xrOrigin;

    public float distance = 1.5f;
    public float height = 0f;

    public Canvas menuCanvas;
    public float fadeSpeed = 5f;
    public float moveThreshold = 0.05f;
    public float followSpeed = 6f;

    public float forwardExtraDistance = 0.5f;
    public float distanceLerpSpeed = 6f;

    [SerializeField] GameObject grabIcon;
    [SerializeField] private Button pinButton;

    private Vector3 lastPosition;
    private float currentDistance;
    private bool opening = false;
    private bool isPinned = true;

    // Offset in XR Origin's LOCAL space, so it rotates with the player
    private Vector3 localOffset;
    private bool isGrabbed = false;
    private ColorBlock defaultPinButtonColors;
    private ColorBlock selectedPinButtonColors;

    void Start()
    {
        lastPosition = xrOrigin.position;
        currentDistance = distance;
        isPinned = true;
        RecalculateOffsetFromForward();
        defaultPinButtonColors = pinButton.colors;
        selectedPinButtonColors = pinButton.colors;
        selectedPinButtonColors.normalColor = selectedPinButtonColors.pressedColor;
        selectedPinButtonColors.highlightedColor = selectedPinButtonColors.pressedColor;
    }

    // Call this from XRGrabInteractable's OnSelectEntered event
    public void OnGrabbed()
    {
        isGrabbed = true;
        //isPinned = false; // Let the XR system move it freely while held
    }

    // Call this from XRGrabInteractable's OnSelectExited event
    public void OnReleased()
    {
        isGrabbed = false;
        //isPinned = true;

        // Snapshot the object's current world position as a new local offset
        // relative to the XR Origin — this becomes the new follow anchor
        Vector3 worldOffset = transform.position - xrOrigin.position;
        localOffset = xrOrigin.InverseTransformDirection(worldOffset);
    }

    public void TogglePin()
    {
        isPinned = !isPinned;

        if (isPinned)
        {
            RecalculateOffsetFromForward(); // Reset to front if re-pinning
            pinButton.colors = defaultPinButtonColors;
        }
        else
        {
            pinButton.colors = selectedPinButtonColors;
        }
    }

    public void OpenMenu()
    {
        opening = true;
        menuCanvas.enabled = false;
        isPinned = true;

        RecalculateOffsetFromForward();

        Vector3 spawnPos = xrOrigin.position + xrOrigin.TransformDirection(localOffset);
        transform.position = spawnPos;

        FacePlayer();
    }

    void LateUpdate()
    {
        if (isPinned && !isGrabbed)
        {
            Vector3 velocity = (xrOrigin.position - lastPosition) / Time.deltaTime;
            lastPosition = xrOrigin.position;

            bool isMoving = velocity.magnitude > moveThreshold;

            // Convert the stored local offset back to world space each frame
            // so the panel orbits correctly as the player rotates
            Vector3 currentWorldOffset = xrOrigin.TransformDirection(localOffset);

            // Optional: push the panel slightly further when walking forward
            Vector3 flatForward = xrOrigin.forward;
            flatForward.y = 0f;
            flatForward.Normalize();

            float forwardDot = 0f;
            if (velocity.sqrMagnitude > 0.0001f)
                forwardDot = Vector3.Dot(velocity.normalized, flatForward);

            bool movingForward = forwardDot > 0.5f;

            if (movingForward && isMoving)
            {
                // Nudge the offset outward along the flat forward axis temporarily
                currentWorldOffset += flatForward * (forwardExtraDistance * 2f);
            }

            Vector3 targetPos = xrOrigin.position + currentWorldOffset;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSpeed
            );

            FacePlayer();

            if (opening)
            {
                StartCoroutine(EnableNextFrame());
                opening = false;
                return;
            }

            SetVisible(!isMoving);
        }
    }

    // Sets localOffset so the panel sits directly in front at the configured distance/height
    private void RecalculateOffsetFromForward()
    {
        Vector3 forward = xrOrigin.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 worldOffset = forward * distance + Vector3.up * height;
        localOffset = xrOrigin.InverseTransformDirection(worldOffset);
    }

    private void FacePlayer()
    {
        Vector3 lookDir = transform.position - xrOrigin.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(lookDir);
    }

    IEnumerator EnableNextFrame()
    {
        yield return null;
        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        menuCanvas.enabled = visible;
        this.GetComponent<SkinnedMeshRenderer>().enabled = visible;
    }
}