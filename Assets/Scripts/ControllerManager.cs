using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] DesktopController desktop;
    [SerializeField] VRController vr;
    [SerializeField] GameObject homePos;
    bool isVRMode;

    [Header("Position logging")]
    [SerializeField] private float positionLogIntervalSeconds = 1f;
    private float positionLogTimer;

    void OnEnable()
    {
        XRModeSwitcher.OnModeSelected += OnModeChosen;
    }

    void OnDisable()
    {
        XRModeSwitcher.OnModeSelected -= OnModeChosen;
    }
    private void OnModeChosen(bool isVR)
    {
        isVRMode = isVR;
    }

    void Update()
    {
        positionLogTimer += Time.unscaledDeltaTime;
        if (positionLogTimer >= positionLogIntervalSeconds)
        {
            positionLogTimer = 0f;
            TelemetryLogger.Instance.LogPosition(GetPosition());
        }
    }

    public float GetHeight()
    {
        if (isVRMode)
            return vr.GetHeight();
        else
            return desktop.GetHeight();
    }

    public void MoveToHotspot(HotspotScript hotspot)
    {
        if (isVRMode)
            vr.MoveToHotspot(hotspot);
        else
            desktop.MoveToHotspot(hotspot);
    }

    public void TakeScreenhot()
    {
        if(isVRMode)
            vr.TakeScreenhot();
        else
            desktop.TakeScreenhot();
    }

    public void ChangeMovementMode()
    {
        if(isVRMode)
            vr.toggleFly();
        else
            desktop.ChangeMovementMode();
    }

    public void MoveToHomePosition()
    {
        if(isVRMode)
            vr.MoveToHomePosition(homePos.transform.position);
        else
            desktop.MoveToHomePosition(homePos.transform.position);
    }

    public Vector3 GetPosition()
    {
        if(isVRMode)
            return vr.GetPosition();
        else
            return desktop.GetPosition();
    }
}
