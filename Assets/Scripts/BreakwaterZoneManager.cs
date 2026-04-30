using UnityEngine;

public class BreakwaterZoneManager : MonoBehaviour
{
    [SerializeField] GameObject breakwater;
    // [SerializeField] Material clipMaterial;
    [SerializeField] HotspotManager hotspotmanager;

    [SerializeField] BreakwaterManager breakwaterManager;
    private string currentSelection = "";
    private bool hasSelection = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // clipMaterial.SetFloat("_min", -700);
        // clipMaterial.SetFloat("_max", 20);
        // clipMaterial.SetFloat("_highlightStrength",0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetHasSelection()
    {
        return hasSelection;
    }

    public string GetSelection()
    {
        return currentSelection;
    }

    public Zone GetSelectionZone()
    {
        return breakwaterManager.GetZone(currentSelection);
    }

    public void ChangeBreakwaterZone(string newZone)
    {
        currentSelection = newZone;
        switch (newZone)
        {
            case "A":
                hasSelection = true;
                break;
            case "B":
                hasSelection = true;
                break;
            case "C":
                hasSelection = true;
                break;
            case "D":
                hasSelection = true;
                break;
            default:
                hasSelection = false;
                newZone = "";
                break;
        }
        breakwaterManager.HideZone(newZone);
        hotspotmanager.ShowOnlyZoneHotspots(newZone);
    }
}
