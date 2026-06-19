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
    }

    public void MenuButton()
    {
        UIManager.OpenMenu();
    }

    public void MovementModeButton()
    {
        ControllerManager.ChangeMovementMode();
        if (movementModeImage.sprite == walkSprite)
            movementModeImage.sprite = flySprite;
        else
            movementModeImage.sprite = walkSprite;
    }

    public void MoveToHotspotMenu()
    {
        UIManager.OpenHotspotChangeMenu();
    }

    public void MoveToHome()
    {
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
}
