using UnityEngine;

public class SnapMenuToPlayer : MonoBehaviour
{
    public Transform xrOrigin;

    public float distance = 1.5f;
    public float height = 0f;

    public Canvas menuCanvas;
    public float fadeSpeed = 5f;
    public float moveThreshold = 0.05f;
    public float followSpeed = 6f;
    private Vector3 frozenTargetPos;
    private float freezeTimer = 0f;
    public float forwardFreezeTime = 0.15f;

    public float forwardExtraDistance = 0.5f;
    public float distanceLerpSpeed = 6f;

    private Vector3 lastPosition;
    private float currentDistance;

    void Start()
    {
        lastPosition = xrOrigin.position;
        currentDistance = distance;
    }

    // void LateUpdate()
    // {
    //     Vector3 velocity = (xrOrigin.position - lastPosition) / Time.deltaTime;
    //     lastPosition = xrOrigin.position;

    //     bool isMoving = velocity.magnitude > moveThreshold;

    //     Vector3 forward = xrOrigin.forward;
    //     forward.y = 0f;
    //     forward.Normalize();

    //     Vector3 right = xrOrigin.right;

    //     // If moving → push menu to the side
    //     Vector3 offset = (forward * distance); 
    //     //     ? (forward * distance + right * sideOffset) 
    //     //     : (forward * distance);
    //     float targetAlpha = isMoving ? 0f : 1f;

    //     //canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    //     SetVisible(!isMoving);

    //     Vector3 targetPos = xrOrigin.position + offset + Vector3.up * height;

    //     transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

    //     // Face player
    //     Vector3 lookDir = transform.position - xrOrigin.position;
    //     lookDir.y = 0f;
    //     transform.rotation = Quaternion.LookRotation(lookDir);
    // }
    void LateUpdate()
    {
        // Calculate velocity
        Vector3 velocity = (xrOrigin.position - lastPosition) / Time.deltaTime;
        lastPosition = xrOrigin.position;

        bool isMoving = velocity.magnitude > moveThreshold;

        // Get flat forward direction
        Vector3 forward = xrOrigin.forward;
        forward.y = 0f;
        forward.Normalize();

        // Detect forward movement
        float forwardDot = 0f;
        if (velocity.sqrMagnitude > 0.0001f)
        {
            forwardDot = Vector3.Dot(velocity.normalized, forward);
        }

        bool movingForward = forwardDot > 0.5f;

        float targetDistance = distance;

        if (movingForward && isMoving)
        {
            targetDistance = distance + forwardExtraDistance*2;
        }

        // Smooth distance change
        //currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * distanceLerpSpeed);
        currentDistance = targetDistance;
        // Position
        Vector3 targetPos = xrOrigin.position + forward * currentDistance + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);

        // Face player
        Vector3 lookDir = transform.position - xrOrigin.position;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir);

        // Visibility (your original logic)
        SetVisible(!isMoving);
    }

    void SetVisible(bool visible)
    {
        menuCanvas.enabled = visible;
    }
}
