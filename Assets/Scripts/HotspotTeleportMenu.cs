using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class HotspotTeleportMenu : MonoBehaviour
{
    [SerializeField] private ControllerManager controllerManager;
    [SerializeField] private TMP_Dropdown dropdown;

    private Dictionary<int, HotspotScript> hotspotMap = new();

    public void PrepareMenu(List<HotspotScript> hotspots)
    {
        hotspotMap.Clear();
        dropdown.ClearOptions();

        foreach (HotspotScript hotspot in hotspots)
        {
            hotspotMap[hotspot.hotspotID] = hotspot;
            dropdown.options.Add(new TMP_Dropdown.OptionData(hotspot.hotspotID.ToString()));
        }
    }

    public void OnClick()
    {
        if (!int.TryParse(dropdown.options[dropdown.value].text, out int selectedID))
        {
            Debug.LogWarning("Failed to parse selected hotspot ID.");
            return;
        }

        if (!hotspotMap.TryGetValue(selectedID, out HotspotScript selectedHotspot))
        {
            Debug.LogWarning($"Hotspot with ID {selectedID} not found.");
            return;
        }

        controllerManager.MoveToHotspot(selectedHotspot);
    }
}