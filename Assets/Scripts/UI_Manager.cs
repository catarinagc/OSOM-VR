using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    GameObject activeUI;
    [SerializeField] GameObject hotspotImageObj;
    [SerializeField] GameObject menuObj;
    [SerializeField] VRController VRController;
    [SerializeField] GameObject zoneMenuObj;
    [SerializeField] GameObject riskMenuObj;
    [SerializeField] GameObject zoneInfoMenuObj;
    [SerializeField] GameObject zoneInspectionMenuObj;
    [SerializeField] GameObject breakwaterMenu;
    [SerializeField] BreakwaterZoneManager breakwaterZoneManager;
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

    public void OpenZoneMenu()
    {
        if (breakwaterZoneManager.GetHasSelection())
        {
            if(activeUI)
                activeUI.SetActive(false);
            activeUI = zoneMenuObj;
            zoneMenuObj.SetActive(true);
            zoneMenuObj.GetComponent<zoneUIManager>().PrepareOpen(breakwaterZoneManager.GetSelectionZone());
        }
    }

    public void OpenRiskMenu(Zone zone)
    {
        zoneMenuObj.SetActive(false);
        activeUI = riskMenuObj;
        riskMenuObj.SetActive(true);
        riskMenuObj.GetComponent<RiskMenuUI_Manager>().PrepareOpen(zone);
    }

    public void OpenZoneInfoMenu(Zone zone)
    {
        zoneMenuObj.SetActive(false);
        activeUI = zoneInfoMenuObj;
        zoneInfoMenuObj.SetActive(true);
        zoneInfoMenuObj.GetComponent<ZoneInfoUI_Manager>().PrepareOpen(zone);
    }

    public void OpenZoneInspectionMenu(Zone zone)
    {
        zoneMenuObj.SetActive(false);
        activeUI = zoneInspectionMenuObj;
        zoneInspectionMenuObj.SetActive(true);
        zoneInspectionMenuObj.GetComponent<ZoneInspectionsUI_Manager>().PrepareOpen(zone);
    }
}
