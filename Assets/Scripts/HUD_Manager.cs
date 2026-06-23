using UnityEngine;
using UnityEngine.UI;
public class HUD_Manager : MonoBehaviour
{

    [SerializeField] UI_Manager UIManager;
    [SerializeField] ControllerManager ControllerManager;
    [SerializeField] BreakwaterManager BreakwaterManager;

    [SerializeField] Sprite walkSprite;
    [SerializeField] Sprite flySprite;
    [SerializeField] Sprite highlightOnSprite;
    [SerializeField] Sprite highlightOffSprite;
    [SerializeField] Image movementModeImage;
    [SerializeField] Image highlightModeImage;

    public void ScreenshotButton()
    {
        ControllerManager.TakeScreenhot();
        TelemetryLogger.Instance.LogUIInteraction("Screenshot", "HUD");
    }

    public void MenuButton()
    {
        UIManager.OpenMenu();
        TelemetryLogger.Instance.LogUIInteraction("Open Menu", "HUD");
    }

    public void MovementModeButton()
    {
        ControllerManager.ChangeMovementMode();
        TelemetryLogger.Instance.LogUIInteraction("Movement Mode", "HUD");
    }

    public void MoveToHotspotMenu()
    {
        UIManager.OpenHotspotChangeMenu();
        TelemetryLogger.Instance.LogUIInteraction("Hotspot Change", "HUD");
    }

    public void MoveToHome()
    {
        TelemetryLogger.Instance.LogUIInteraction("Click Home");
        ControllerManager.MoveToHomePosition();
    }

    public void ToggleHighlights()
    {
        BreakwaterManager.ToggleHighlights();
        if (highlightModeImage.sprite == highlightOnSprite)
            highlightModeImage.sprite = highlightOffSprite;
        else
            highlightModeImage.sprite = highlightOnSprite;
    }

    public void setMovementModeIsFly(bool isFlying)
    {
        if (isFlying)
            movementModeImage.sprite = walkSprite;
        else
            movementModeImage.sprite = flySprite;
    }
}
