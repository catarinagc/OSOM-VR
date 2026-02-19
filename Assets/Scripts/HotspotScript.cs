using UnityEngine;

public class HotspotScript : MonoBehaviour
{
    [SerializeField] public Vector2 realWorldPosition;
    [SerializeField] GameObject hotspotImageObj;
    private bool isActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract()
    {
        isActive = !isActive;
        if (isActive)
        {
            Debug.Log("Hotspot Clicked");  
            hotspotImageObj.SetActive(true);
        }
        else
        {
            hotspotImageObj.SetActive(false);
        }
    }
}
