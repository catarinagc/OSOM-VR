using UnityEngine;
using System.Collections.Generic;
public class UI_Manager : MonoBehaviour
{
    private List<GameObject> activeUIs;
    [SerializeField] GameObject hotspotImageObj;
    [SerializeField] GameObject menuObj;
    [SerializeField] VRController VRController;
    [SerializeField] GameObject zoneMenuObj;
    [SerializeField] GameObject riskMenuObj;
    [SerializeField] GameObject zoneRiskSelectorObj;
    [SerializeField] GameObject zoneInfoMenuObj;
    [SerializeField] GameObject zoneInfoSelectorObj;
    [SerializeField] GameObject zoneInspectionMenuObj;
    [SerializeField] GameObject zoneInspectionSelectorObj;
    [SerializeField] GameObject breakwaterMenu;
    [SerializeField] BreakwaterZoneManager breakwaterZoneManager;
    [SerializeField] Transform spawnPoint;
    private bool isVR = false;

    void Awake()
    {
        activeUIs = new List<GameObject>();
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
        this.isVR = isVR;
    }

    public void openHotspotImageUI(int hotspotID, char troco_ID, List<InspectionImage> images)
    {
        CloseActiveUIs();
        hotspotImageObj.GetComponent<Image_UI_Manager>().OnModeChosen(isVR);
        Debug.Log($"images null? {images == null} | count: {images?.Count}");
        hotspotImageObj.GetComponent<Image_UI_Manager>().PrepareOpen(hotspotID, troco_ID, images);
        activeUIs.Add(hotspotImageObj);
        hotspotImageObj.SetActive(true);
    }

    public void CloseActiveUIs()
    {
        if (activeUIs == null || activeUIs.Count == 0)
            return;
        
        foreach (GameObject ui in activeUIs)
        {
            if (ui == hotspotImageObj)
                ui.GetComponent<Image_UI_Manager>().Close();
            ui.SetActive(false);
        }

        activeUIs.Clear();

        if (VRController)
            VRController.stopInteraction();
    }

    private void CloseSpecificUI(GameObject openUI)
    {
        if (activeUIs.Contains(openUI))
        {
            openUI.SetActive(false);
            activeUIs.Remove(openUI);
        }
    }

    public void OpenMenu()
    {
        CloseActiveUIs();
        activeUIs.Add(menuObj);
        menuObj.SetActive(true);
    }

    public void OpenZoneMenu()
    {
        if (breakwaterZoneManager.GetHasSelection())
        {
            CloseActiveUIs();
            activeUIs.Add(zoneMenuObj);
            zoneMenuObj.SetActive(true);
            zoneMenuObj.GetComponent<zoneUIManager>().PrepareOpen(breakwaterZoneManager.GetSelectionZone());
        }
    }

    public void OpenRiskMenu(Zone zone)
    {
        CloseActiveUIs();
        activeUIs.Add(riskMenuObj);
        if (isVR)
        {      
            activeUIs.Add(zoneRiskSelectorObj);
            zoneRiskSelectorObj.SetActive(true);
            riskMenuObj.GetComponent<SnapMenuToPlayer>().OpenMenu();
        }
        riskMenuObj.SetActive(true);
        riskMenuObj.GetComponent<RiskMenuUI_Manager>().PrepareOpen(zone);
    }

    public void OpenZoneInfoMenu(Zone zone)
    {
        CloseActiveUIs();
        activeUIs.Add(zoneInfoMenuObj);
        if (isVR)
        {      
            activeUIs.Add(zoneInfoSelectorObj);
            zoneInfoSelectorObj.SetActive(true);
            zoneInfoMenuObj.transform.SetParent(spawnPoint, false);
            zoneInfoMenuObj.transform.localPosition = Vector3.zero;
            zoneInfoMenuObj.transform.localRotation = Quaternion.identity;
            zoneInfoMenuObj.GetComponent<SnapMenuToPlayer>().OpenMenu();
        }
        zoneInfoMenuObj.SetActive(true);
        zoneInfoMenuObj.GetComponent<ZoneInfoUI_Manager>().PrepareOpen(zone);
    }

    public void OpenZoneInspectionMenu(Zone zone)
    {
        CloseActiveUIs();
        activeUIs.Add(zoneInspectionMenuObj);
        zoneInspectionMenuObj.SetActive(true);
        if (isVR)
        {
            activeUIs.Add(zoneInspectionSelectorObj);
            zoneInspectionSelectorObj.SetActive(true);
            zoneInspectionMenuObj.GetComponent<SnapMenuToPlayer>().OpenMenu();
        }
        zoneInspectionMenuObj.GetComponent<ZoneInspectionsUI_Manager>().PrepareOpen(zone, zone.lastInspection.Year);
    }

    public void OpenZoneInspectionRefMenu(Zone zone)
    {
        CloseActiveUIs();
        activeUIs.Add(zoneInspectionMenuObj);
        zoneInspectionMenuObj.SetActive(true);
        if (isVR)
        {
            activeUIs.Add(zoneInspectionSelectorObj);
            zoneInspectionSelectorObj.SetActive(true);
            zoneInspectionMenuObj.GetComponent<SnapMenuToPlayer>().OpenMenu();
        }
        zoneInspectionMenuObj.GetComponent<ZoneInspectionsUI_Manager>().PrepareOpen(zone, zone.referenceInspection.Year);
    }
}
