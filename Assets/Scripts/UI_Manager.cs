using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class UI_Manager : MonoBehaviour
{
    private List<GameObject> activeUIs;
    private List<GameObject> persistentVRMenus; // content menus that stay open in VR
    private List<GameObject> handVRMenus;
    [SerializeField] GameObject hotspotImageObj;
    [SerializeField] GameObject menuObj;
    [SerializeField] VRController VRController;
    [SerializeField] GameObject zoneMenuObj;
    [SerializeField] GameObject riskMenuObj;
    [SerializeField] GameObject zoneInfoMenuObj;
    [SerializeField] GameObject zoneInspectionMenuObj;
    [SerializeField] GameObject zoneRiskSelectorObj;
    [SerializeField] GameObject zoneInfoSelectorObj;
    [SerializeField] GameObject zoneInspectionSelectorObj;
    [SerializeField] GameObject breakwaterMenu;
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject hotspotChangeMenu;
    [SerializeField] GameObject noteMenu;
    [SerializeField] BreakwaterZoneManager breakwaterZoneManager;
    [SerializeField] HotspotManager hotspotManager;
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject vrNoteHandMenu;
    [SerializeField] GameObject vrTasksMenu;
    [SerializeField] GameObject pcTasksMenu;

    private bool isVR = false;
    private bool toReturnMenu = false;

    // Maps each persistent content menu to its selector
    private Dictionary<GameObject, GameObject> contentToSelector;

    void Awake()
    {
        activeUIs = new List<GameObject>();
        persistentVRMenus = new List<GameObject>();
        handVRMenus = new List<GameObject>();

        contentToSelector = new Dictionary<GameObject, GameObject>
        {
            { riskMenuObj,           zoneRiskSelectorObj      },
            { zoneInfoMenuObj,       zoneInfoSelectorObj      },
            { zoneInspectionMenuObj, zoneInspectionSelectorObj }
        };

        handVRMenus.Add(zoneInspectionSelectorObj);
        handVRMenus.Add(zoneRiskSelectorObj);
        handVRMenus.Add(zoneInfoSelectorObj);
        handVRMenus.Add(noteMenu);
        handVRMenus.Add(zoneMenuObj);
        handVRMenus.Add(menuObj);
        handVRMenus.Add(vrNoteHandMenu);
        handVRMenus.Add(hotspotChangeMenu);
        handVRMenus.Add(vrTasksMenu);
    }

    void OnEnable()  { XRModeSwitcher.OnModeSelected += OnModeChosen; }
    void OnDisable() { XRModeSwitcher.OnModeSelected -= OnModeChosen; }

    private void OnModeChosen(bool isVR) 
    { 
        this.isVR = isVR;
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private bool IsPersistentMenu(GameObject ui)
        => contentToSelector.ContainsKey(ui);

    /// <summary>Hide the selector of every persistent menu that is currently open.</summary>
    private void HideAllPersistentSelectors()
    {
        foreach (var menu in persistentVRMenus)
        {
            if (contentToSelector.TryGetValue(menu, out var selector))
                selector.SetActive(false);
        }
    }

    /// <summary>
    /// Toggle the selector for a persistent menu.
    /// If the menu is already persistent (open), just re-show its selector and return true.
    /// </summary>
    private bool TryReopenSelector(GameObject contentMenu)
    {
        if (!isVR || !persistentVRMenus.Contains(contentMenu))
            return false;

        if (contentToSelector.TryGetValue(contentMenu, out var selector))
        {
            selector.SetActive(true);
            activeUIs.Add(selector);
        }

        return true;
    }

    // ── Close logic ────────────────────────────────────────────────────────────

    public void CloseHandMenus()
    {
        foreach (GameObject ui in handVRMenus)
        {
            if (ui != null && ui.activeSelf)
                ui.SetActive(false);
        }
        if (isHotspotActive())
        {
            CloseActiveUIs();
        }
    }
    
    public void CloseActiveUIs()
    {
        if (activeUIs == null || activeUIs.Count == 0)
            return;

        foreach (GameObject ui in activeUIs)
        {
            if (ui == hotspotImageObj)
            {
                ui.GetComponent<Image_UI_Manager>().Close();
            }

            if (isVR && IsPersistentMenu(ui))
            {
                // Keep the content panel alive; just hide its selector
                if (contentToSelector.TryGetValue(ui, out var selector))
                    selector.SetActive(false);

                if (!persistentVRMenus.Contains(ui))
                    persistentVRMenus.Add(ui);

                // Skip SetActive(false) below — continue to next item
                continue;
            }

            // Non-VR persistent menus still trigger the zone-menu return
            if (!isVR && (ui == riskMenuObj || ui == zoneInfoMenuObj || ui == zoneInspectionMenuObj))
                toReturnMenu = true;

            ui.SetActive(false);
        }

        activeUIs.Clear();

        if (VRController)
            VRController.stopInteraction();

        if (toReturnMenu)
        {
            OpenZoneMenu();
            toReturnMenu = false;
        }

        if (!isVR)
            HUD.SetActive(true);
    }

    public void CloseSpecificUI(GameObject openUI)
    {
        if (activeUIs.Contains(openUI))
        {
            openUI.SetActive(false);
            activeUIs.Remove(openUI);
        }
        // Also remove from persistent list if present
        if (persistentVRMenus.Contains(openUI))
        {
            persistentVRMenus.Remove(openUI);
            openUI.SetActive(false);
        }

        if (contentToSelector.TryGetValue(openUI, out var selector))
        {
            selector.SetActive(false);
            if (activeUIs.Contains(selector))
            {
                activeUIs.Remove(selector);
            }
        }
    }

    // ── Open methods ───────────────────────────────────────────────────────────

    public void OpenMenu()
    {
        CloseHandMenus();
        CloseActiveUIs();
        CloseActiveUIs();
        activeUIs.Add(menuObj);
        menuObj.SetActive(true);
        // if (!isVR)
        //     HUD.SetActive(false);
    }

    public void OpenZoneMenu()
    {
        if (!breakwaterZoneManager.GetHasSelection()) return;
        TelemetryLogger.Instance.LogUIInteraction("Open Zone Menu");
        CloseHandMenus();
        CloseActiveUIs();
        //HideAllPersistentSelectors();
        activeUIs.Add(zoneMenuObj);
        zoneMenuObj.SetActive(true);
        zoneMenuObj.GetComponent<zoneUIManager>().PrepareOpen(breakwaterZoneManager.GetSelectionZone());
        // if (!isVR)
        //     HUD.SetActive(false);
    }

    public void OpenRiskMenu(Zone zone)
    {
        CloseHandMenus();
        // If already open in VR, just restore its selector
        if (TryReopenSelector(riskMenuObj)) return;
        TelemetryLogger.Instance.LogUIInteraction("Open Risk Menu");
        CloseActiveUIs();
        activeUIs.Add(riskMenuObj);
        riskMenuObj.SetActive(true);
        riskMenuObj.GetComponent<RiskMenuUI_Manager>().PrepareOpen(zone);

        if (isVR)
        {
            activeUIs.Add(zoneRiskSelectorObj);
            zoneRiskSelectorObj.SetActive(true);
            riskMenuObj.GetComponent<SnapMenuToPlayer>().OpenMenu();
        }
        // else
        // {
        //     HUD.SetActive(false);
        // }
    }

    public void OpenZoneInfoMenu(Zone zone)
    {
        CloseHandMenus();
        if (TryReopenSelector(zoneInfoMenuObj)) return;

        TelemetryLogger.Instance.LogUIInteraction("Open Info Menu");

        CloseActiveUIs();
        activeUIs.Add(zoneInfoMenuObj);
        zoneInfoMenuObj.SetActive(true);
        zoneInfoMenuObj.GetComponent<ZoneInfoUI_Manager>().PrepareOpen(zone);

        if (isVR)
        {
            activeUIs.Add(zoneInfoSelectorObj);
            zoneInfoSelectorObj.SetActive(true);
            zoneInfoMenuObj.transform.SetParent(spawnPoint, false);
            zoneInfoMenuObj.transform.localPosition = Vector3.zero;
            zoneInfoMenuObj.transform.localRotation = Quaternion.identity;
            zoneInfoMenuObj.GetComponent<SnapMenuToPlayer>().OpenMenu();
        }
        // else {
        //     HUD.SetActive(false);
        // }
    }

    public void OpenZoneInspectionMenu(Zone zone)
    {
        CloseHandMenus();
        if (TryReopenSelector(zoneInspectionMenuObj)) return;

        TelemetryLogger.Instance.LogUIInteraction("Open Inspection Menu");

        CloseActiveUIs();
        activeUIs.Add(zoneInspectionMenuObj);
        zoneInspectionMenuObj.SetActive(true);
        zoneInspectionMenuObj.GetComponent<ZoneInspectionsUI_Manager>().PrepareOpen(zone, zone.lastInspection.Year);

        if (isVR)
        {
            activeUIs.Add(zoneInspectionSelectorObj);
            zoneInspectionSelectorObj.SetActive(true);
            zoneInspectionMenuObj.GetComponent<SnapMenuToPlayer>().OpenMenu();
        }
        // else
        // {
        //     HUD.SetActive(false);
        // }
        
    }

    public void OpenZoneInspectionRefMenu(Zone zone)
    {
        CloseHandMenus();
        if (TryReopenSelector(zoneInspectionMenuObj)) return;
        
        TelemetryLogger.Instance.LogUIInteraction("Open Inspection Menu");

        CloseActiveUIs();
        activeUIs.Add(zoneInspectionMenuObj);
        zoneInspectionMenuObj.SetActive(true);
        zoneInspectionMenuObj.GetComponent<ZoneInspectionsUI_Manager>().PrepareOpen(zone, zone.referenceInspection.Year);

        if (isVR)
        {
            activeUIs.Add(zoneInspectionSelectorObj);
            zoneInspectionSelectorObj.SetActive(true);
            zoneInspectionMenuObj.GetComponent<SnapMenuToPlayer>().OpenMenu();
        }
    }

    public void openHotspotImageUI(int hotspotID, char troco_ID, List<InspectionImage> images)
    {
        TelemetryLogger.Instance.LogUIInteraction("Open Hotspot");
        hotspotImageObj.GetComponent<Image_UI_Manager>().OnModeChosen(isVR);
        Debug.Log($"images null? {images == null} | count: {images?.Count}");
        hotspotImageObj.GetComponent<Image_UI_Manager>().PrepareOpen(hotspotID, troco_ID, images);

    }

    public void ShowLoadingScreen()
    {
        CloseActiveUIs();
        HideAllPersistentSelectors();
        if (!isVR)
        {
            HUD.SetActive(false);
            CloseTaskMenu();
        }
        Debug.Log($"[Loading] SetActive(true) at frame {Time.frameCount}");
        hotspotImageObj.SetActive(true);
        hotspotImageObj.GetComponent<Image_UI_Manager>().loadingImagesScreen.SetActive(true);
        activeUIs.Add(hotspotImageObj);
    }

    public void OpenHotspotChangeMenu()
    {
        if (isVR)
            CloseHandMenus();
        else
        {
            CloseActiveUIs();
            CloseActiveUIs();
        }

        TelemetryLogger.Instance.LogUIInteraction("Open Hotspot Move to");
        activeUIs.Add(hotspotChangeMenu);
        hotspotChangeMenu.SetActive(true);
        if (!hotspotChangeMenu.GetComponent<HotspotTeleportMenu>().isPrepared)
        {
            List<HotspotScript> hotspots = hotspotManager.GetHotspotList();
            hotspotChangeMenu.GetComponent<HotspotTeleportMenu>().PrepareMenu(hotspots);
        }
    }

    public void ReopenSelectorForMenu(GameObject contentMenu)
    {
        CloseHandMenus();
        if (!isVR) return;
        if (contentToSelector.TryGetValue(contentMenu, out var selector))
        {
            selector.SetActive(true);
            activeUIs.Add(selector);
        }
    }

    public void OpenNoteMenu()
    {
        TelemetryLogger.Instance.LogUIInteraction("Open Note Menu");
        CloseHandMenus();
        vrNoteHandMenu.SetActive(true);
        activeUIs.Add(vrNoteHandMenu);
    }

    public void OpenTasksMenu()
    {
        if (isVR)
        {
            CloseHandMenus();
            vrTasksMenu.SetActive(true);
            activeUIs.Add(vrTasksMenu);
        }else
        {
            //CloseActiveUIs();
            //CloseActiveUIs();
            pcTasksMenu.SetActive(true);
            //activeUIs.Add(pcTasksMenu);
        }
        TelemetryLogger.Instance.LogUIInteraction("Open Task Menu");
    }

    public void setMovementModeIsFly(bool isFlying)
    {
        HUD.GetComponent<HUD_Manager>().setMovementModeIsFly(isFlying);
    }

    public bool isHotspotActive() => hotspotImageObj.active;

    public void CloseTaskMenu()
    {
        if (isVR)
        {
            CloseHandMenus();
        }
        else
        {
            if (pcTasksMenu.active)
                pcTasksMenu.SetActive(false);
        }
    }
}