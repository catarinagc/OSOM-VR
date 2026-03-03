using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Image_Button : MonoBehaviour
{
    [SerializeField] Image_UI_Manager UIManager;
    [SerializeField] private Sprite sourceImage_F;
    [SerializeField] private Sprite sourceImage_T;
    [SerializeField] private Sprite sourceImage_L;
    [SerializeField] Image activeImage;
    [SerializeField] private TMP_Text yearText;

    private void Awake() {
        activeImage.sprite = sourceImage_F;
    }

    public void ChangeActiveImage(Image_UI_Manager.ViewDirection direction)
    {
        switch (direction)
        {
            case Image_UI_Manager.ViewDirection.F:
                activeImage.sprite = sourceImage_F;
                break;

            case Image_UI_Manager.ViewDirection.L:
                activeImage.sprite = sourceImage_L;
                break;

            case Image_UI_Manager.ViewDirection.T:
                activeImage.sprite = sourceImage_T;
                break;
            default:
                break;
        }
    }
    public void OnClick()
    {
        UIManager.ShowItem(activeImage.sprite, yearText.text);
        //UIManager.SetImageFullscreen(activeImage.sprite);
    }
}
