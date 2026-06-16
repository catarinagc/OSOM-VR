using UnityEngine;

public class BeltHUDFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform playerCamera; // Your VR camera/head transform

    [Header("Positioning")]
    public float followDistance = 1.2f;
    public float verticalOffset = -0.4f;   // Below eye level = belt feel
    public float heightFromGround = 1.0f;  // Optional: fix to world height

    [Header("Smoothing")]
    public float rotationSpeed = 3f;   // How quickly it catches up
    public float deadZoneAngle = 30f;  // Angle before HUD starts following

    private float _targetYaw;
    private float _currentYaw;

    void Start()
    {
        _currentYaw = transform.eulerAngles.y;
        _targetYaw  = _currentYaw;
    }

    void Update()
    {
        float cameraYaw = playerCamera.eulerAngles.y;
        float angleDiff = Mathf.DeltaAngle(_currentYaw, cameraYaw);

        if (Mathf.Abs(angleDiff) > deadZoneAngle)
            _targetYaw = cameraYaw - Mathf.Sign(angleDiff) * deadZoneAngle;

        _currentYaw = Mathf.LerpAngle(_currentYaw, _targetYaw, Time.deltaTime * rotationSpeed);

        // Position uses yaw-only rotation so X tilt doesn't affect placement
        Quaternion yawOnly = Quaternion.Euler(0, _currentYaw, 0);
        Vector3 forward = yawOnly * Vector3.forward;

        transform.position = playerCamera.position
            + forward * followDistance
            + Vector3.up * verticalOffset;

        // Rotation applies the fixed 14° tilt on top of the yaw
        transform.rotation = Quaternion.Euler(14, _currentYaw, 0);
    }
}