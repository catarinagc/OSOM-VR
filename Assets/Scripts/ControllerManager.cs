using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] DesktopController desktop;
    [SerializeField] VRController vr;
    bool isVRMode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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

    public float GetHeight()
    {
        if (isVRMode)
            return vr.GetHeight();
        else
            return desktop.GetHeight();
    }
}
