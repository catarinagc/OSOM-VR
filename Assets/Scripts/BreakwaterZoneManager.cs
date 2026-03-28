using UnityEngine;

public class BreakwaterZoneManager : MonoBehaviour
{
    [SerializeField] GameObject breakwater;
    [SerializeField] Material clipMaterial;
    [SerializeField] HotspotManager hotspotmanager;
    private string currentSelection = "";
    private bool hasSelection = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clipMaterial.SetFloat("_min", -128);
        clipMaterial.SetFloat("_max", 0);
        clipMaterial.SetFloat("_highlightStrength",0f);
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

    public void ChangeBreakwaterZone(string newZone)
    {
        currentSelection = newZone;
        switch (newZone)
        {
            case "A":
                clipMaterial.SetFloat("_min", -128);
                clipMaterial.SetFloat("_max", -83);
                clipMaterial.SetFloat("_highlightStrength",0.25f);
                hasSelection = true;
                break;
            case "B":
                clipMaterial.SetFloat("_min", -83);
                clipMaterial.SetFloat("_max", -51);
                clipMaterial.SetFloat("_highlightStrength",0.25f);
                hasSelection = true;
                break;
            case "C":
                clipMaterial.SetFloat("_min", -51);
                clipMaterial.SetFloat("_max", -14);
                clipMaterial.SetFloat("_highlightStrength",0.25f);
                hasSelection = true;
                break;
            case "D":
                clipMaterial.SetFloat("_min", -14);
                clipMaterial.SetFloat("_max", 0);
                clipMaterial.SetFloat("_highlightStrength",0.25f);
                hasSelection = true;
                break;
            default:
                clipMaterial.SetFloat("_min", -128);
                clipMaterial.SetFloat("_max", 0);
                clipMaterial.SetFloat("_highlightStrength",0.0f);
                hasSelection = false;
                newZone = "";
                break;
        }
        hotspotmanager.ShowOnlyZoneHotspots(newZone);
    }
}
