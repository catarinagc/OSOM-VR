using UnityEngine;

public class HotspotDirectionButton : MonoBehaviour
{
    [SerializeField] Image_UI_Manager UIManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Image_UI_Manager.ViewDirection direction;

    public void OnClick()
    {
        UIManager.ChangeViewDirection(direction);
    }
}
