using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    GameObject activeUI;
    [SerializeField] GameObject hotspotImageObj;
    [SerializeField] VRController VRController;
    private bool isVR = false;

    void OnEnable()
    {
        XRModeSwitcher.OnModeSelected += OnModeChosen;
    }

    void OnDisable()
    {
        XRModeSwitcher.OnModeSelected -= OnModeChosen;
    }

    private void OnModeChosen(bool isVR)
    {
        this.isVR = isVR;
    }
    //public void setActiveUI(GameObject active)
    //{
    //    activeUI = active;
    //}

    public void openHotspotImageUI(int hotspotID, char troco_ID)
    {
        hotspotImageObj.GetComponent<Image_UI_Manager>().PrepareOpen(hotspotID, troco_ID);
        hotspotImageObj.GetComponent<Image_UI_Manager>().OnModeChosen(isVR);
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
