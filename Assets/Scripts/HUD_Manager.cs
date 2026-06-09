using UnityEngine;

public class HUD_Manager : MonoBehaviour
{

    [SerializeField] UI_Manager UIManager;
    [SerializeField] ControllerManager ControllerManager;
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
    }

    public void MoveToHotspotMenu()
    {
        UIManager.OpenHotspotChangeMenu();
    }

    public void MoveToHome()
    {
        ControllerManager.MoveToHomePosition();
    }
}
