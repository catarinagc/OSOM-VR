using UnityEngine;

public class HotspotClearImage : MonoBehaviour
{
    [SerializeField] Image_UI_Manager UIManager;

    public bool isFirstSlot;
    public void OnClick()
    {
        UIManager.HideItem(isFirstSlot);
    }
}
