using UnityEngine;

public class CloseImageVR : MonoBehaviour
{
    [SerializeField] GameObject rootObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnCLick()
    {
        rootObj.SetActive(false);
    }
}
