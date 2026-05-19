using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class zoneUIManager : MonoBehaviour
{
    [System.Serializable]
    public class ZoneUIField
    {
        public string key;      // identifier (e.g. "Manto", "Tardoz")
        public TMP_Text text;
    }

    [SerializeField] UI_Manager ui_manager;
    [SerializeField] List<ZoneUIField> fields;
    private string default_title_text;

    private Zone currentZone;

    void Awake()
    {
        default_title_text = "Portimão Poente ";
    }

    public void PrepareOpen(Zone zone)
    {
        currentZone = zone;

        var data = zone.GetUIData();

        foreach (var field in fields)
        {
            if (data.ContainsKey(field.key))
            {
                field.text.text = data[field.key];
            }
            else
            {
                field.text.text = "-";
            }
        }
    }

    public void OpenRiskMenu()
    {
        ui_manager.OpenRiskMenu(currentZone);
        currentZone = null;
    }

    public void OpenZoneInfoMenu()
    {
        ui_manager.OpenZoneInfoMenu(currentZone);
        currentZone = null;
    }

    public void OpenZoneInspectionMenu()
    {
        ui_manager.OpenZoneInspectionMenu(currentZone);
        currentZone = null;
    }

    public void OpenZoneInspectionRefMenu()
    {
        ui_manager.OpenZoneInspectionRefMenu(currentZone);  
        currentZone = null;
    }
}
