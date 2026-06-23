using UnityEngine;
using TMPro;
public class CollapsableUI : MonoBehaviour
{
    [SerializeField] GameObject collapse_UI;
    [SerializeField] GameObject total_uis;
    [SerializeField] TMP_Text ui_title;
    [SerializeField] TMP_Text selected_title;
    [SerializeField] GameObject sprite;

    public void OnClick()
    {
        sprite.transform.Rotate(0, 0, 180);
        if (collapse_UI.active)
            collapse_UI.SetActive(false);
        else
            collapse_UI.SetActive(true);
    }

    public void OnClickVR()
    {
        foreach (Transform child in total_uis.transform)
        {
            child.gameObject.SetActive(false);
        }
        collapse_UI.SetActive(true);
        ui_title.text = selected_title.text;
        TelemetryLogger.Instance.LogUIInteraction("Interact with submenu", selected_title.text);
    }
}
