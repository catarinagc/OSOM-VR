using UnityEngine;
using System.Collections;

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
    private bool hasFullyAppeared = false;
    private float settleTimer = 0f;
    public float settleTime = 0.2f;
    private bool opening = false;

    void Start()
    {
        lastPosition = xrOrigin.position;
        currentDistance = distance;
    }

    public void OpenMenu()
    {
        opening = true;
        menuCanvas.enabled = false;

        // ✅ Snap menu instantly to the correct position in front of player
        Vector3 forward = xrOrigin.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 spawnPos = xrOrigin.position + forward * distance + Vector3.up * height;
        transform.position = spawnPos;

        // ✅ Face player immediately
        Vector3 lookDir = spawnPos - xrOrigin.position;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir);
    }

    void LateUpdate()
    {
        Vector3 velocity =
            (xrOrigin.position - lastPosition) / Time.deltaTime;

        lastPosition = xrOrigin.position;

        bool isMoving = velocity.magnitude > moveThreshold;

        Vector3 forward = xrOrigin.forward;
        forward.y = 0f;
        forward.Normalize();

        float forwardDot = 0f;

        if (velocity.sqrMagnitude > 0.0001f)
            forwardDot = Vector3.Dot(velocity.normalized, forward);

        bool movingForward = forwardDot > 0.5f;

        float targetDistance = distance;

        if (movingForward && isMoving)
            targetDistance = distance + forwardExtraDistance * 2;

        currentDistance = targetDistance;

        Vector3 targetPos =
            xrOrigin.position +
            forward * currentDistance +
            Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * followSpeed
        );

        Vector3 lookDir = transform.position - xrOrigin.position;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir);

        if (opening)
        {
            StartCoroutine(EnableNextFrame());
            opening = false;
            return;
        }

        SetVisible(!isMoving);
    }

    IEnumerator EnableNextFrame()
    {
        yield return null;
        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        menuCanvas.enabled = visible;
    }
}