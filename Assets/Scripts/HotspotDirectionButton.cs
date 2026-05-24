using UnityEngine;
using TMPro;
public class HotspotDirectionButton : MonoBehaviour
{
    [SerializeField] Image_UI_Manager UIManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] TMP_Text direction;

    public void OnClick()
    {
        UIManager.ShowDirection(direction.text);
    }
}
