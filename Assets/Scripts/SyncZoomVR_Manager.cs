using UnityEngine;
using System.Collections.Generic;
public class SyncZoomVR_Manager : MonoBehaviour
{
    public List<VRZoomImage> connectedImages = new List<VRZoomImage>();

    public void UpdateAllImages(float zoom, Vector2 pan)
    {
        foreach(VRZoomImage image in connectedImages)
        {
            image.Apply(zoom, pan);
        }
    }

    public void AddImage(VRZoomImage image)
    {
        if (connectedImages.Count !=  0)
        {
            float zoom = connectedImages[0].getZoom();
            Vector2 pan = connectedImages[0].getPan();
            image.Apply(zoom, pan);
        }
        connectedImages.Add(image);
    }

    public void RemoveImage(VRZoomImage image)
    {
        connectedImages.Remove(image);
    }
}
