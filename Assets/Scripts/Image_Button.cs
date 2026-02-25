using UnityEngine;
using UnityEngine.UI;

public class Image_Button : MonoBehaviour
{
    [SerializeField] Image_UI_Manager UIManager;
    
    private Image sourceImage;
    void Awake()
    {
        sourceImage = GetComponent<Image>();
    }
    public void OnClick()
    {
        Debug.Log("Clicked: ");

        UIManager.ShowItem(sourceImage.sprite);
    }
}
