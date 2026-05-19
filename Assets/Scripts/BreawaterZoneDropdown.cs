using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreawaterZoneDropdown : MonoBehaviour
{
    [SerializeField] TMP_Text currentOption;
    [SerializeField] BreakwaterZoneManager zoneManager;

    [SerializeField] Button button;

    [SerializeField] Color disabledColor = Color.gray;

    [SerializeField] string defaultValue = "Total";

    private Color normalColor;

    void Start()
    {
        normalColor = button.image.color;
        button.image.color = disabledColor;
        button.interactable = false;
    }

    public void OnChange()
    {
        zoneManager.ChangeBreakwaterZone(currentOption.text);

        if (currentOption.text == defaultValue)
        {
            button.image.color = disabledColor;
            button.interactable = false;
        }
        else
        {
            button.image.color = normalColor;
            button.interactable = true;
        }
    }
}