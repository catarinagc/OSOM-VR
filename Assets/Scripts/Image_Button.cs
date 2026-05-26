using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Image_Button : MonoBehaviour
{
    [SerializeField] public Image_UI_Manager UIManager;
    [SerializeField] Image activeImage;
    [SerializeField] private TMP_Text yearText;
    [SerializeField] public InspectionImage imageData;

    public void OnClick()
    {
        UIManager.imageInteract(activeImage, yearText.text, imageData);
    }
}
