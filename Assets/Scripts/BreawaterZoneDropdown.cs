using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BreawaterZoneDropdown : MonoBehaviour
{
    [SerializeField] TMP_Text currentOption;
    [SerializeField] BreakwaterZoneManager zoneManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnChange()
    {
        zoneManager.ChangeBreakwaterZone(currentOption.text);
    }
}
