using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    GameObject activeUI;
    [SerializeField] GameObject hotspotImageObj;
    [SerializeField] GameObject menuObj;
    [SerializeField] VRController VRController;
    [SerializeField] GameObject zoneMenuObj;
    private bool isVR = false;

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
        this.isVR = isVR;
    }

    public void openHotspotImageUI(int hotspotID, char troco_ID)
    {
        hotspotImageObj.GetComponent<Image_UI_Manager>().PrepareOpen(hotspotID, troco_ID);
        hotspotImageObj.GetComponent<Image_UI_Manager>().OnModeChosen(isVR);
        activeUI = hotspotImageObj;
        hotspotImageObj.SetActive(true);
    }

    public void CloseActiveUI()
    {
        activeUI.SetActive(false);
        activeUI = null;
        if (VRController)
            VRController.stopInteraction();
    }

    public void OpenMenu()
    {
        activeUI = menuObj;
        menuObj.SetActive(true);
    }

    public void OpenZoneMenu(string zoneSelected)
    {
        activeUI = zoneMenuObj;
        zoneMenuObj.SetActive(true);
        zoneMenuObj.GetComponent<zoneUIManager>().PrepareOpen(zoneSelected);
    }
}
