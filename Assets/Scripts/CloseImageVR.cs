using UnityEngine;

public class CloseImageVR : MonoBehaviour
{
    [SerializeField] GameObject rootObj;
    [SerializeField] VRZoomImage VRZoomImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnCLick()
    {
        VRZoomImage.OnCloseImage();
        rootObj.SetActive(false);
        Destroy(rootObj);
    }
}
