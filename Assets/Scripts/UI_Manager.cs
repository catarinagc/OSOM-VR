using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    GameObject activeUI;
    [SerializeField] GameObject hotspotImageObj;
    [SerializeField] VRController VRController;


    //public void setActiveUI(GameObject active)
    //{
    //    activeUI = active;
    //}

    public void openHotspotImageUI(int hotspotID, char troco_ID)
    {
        hotspotImageObj.GetComponent<Image_UI_Manager>().PrepareOpen(hotspotID, troco_ID);
        activeUI = hotspotImageObj;
        hotspotImageObj.SetActive(true);
    }

    public void CloseActiveUI()
    {
        activeUI.SetActive(false);
        activeUI = null;
        if (VRController != null)
        {
            VRController.stopInteraction();
        }
    }
}
