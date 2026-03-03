using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Image_UI_Manager : MonoBehaviour
{
    [SerializeField] Image imagePlaceholder1;
    [SerializeField] TMP_Text textPlaceholder1;
    [SerializeField] Image imagePlaceholder2;
    [SerializeField] TMP_Text textPlaceholder2;

    [SerializeField] GameObject imagesHolder;

    [SerializeField] GameObject fullscreenPlaceholder;
    public enum ViewDirection
    {
        F,
        L,
        T
    }

    private ViewDirection currentDir;

    private bool useFirstSlot = true;

    void Start()
    {
        currentDir = ViewDirection.F;
    }

    public void ShowItem(Sprite newSprite, string year)
    {
        if (textPlaceholder1.text == year || textPlaceholder2.text == year)
            return;

        if (useFirstSlot)
        {
            imagePlaceholder1.sprite = newSprite;
            textPlaceholder1.text = year;
        }
        else
        {
            imagePlaceholder2.sprite = newSprite;
            textPlaceholder2.text = year;
        }

        useFirstSlot = !useFirstSlot;
    }

    public void HideItem(bool isFirstSlot)
    {
        if (isFirstSlot)
        {
            textPlaceholder1.text = "";
            imagePlaceholder1.sprite = null;
            useFirstSlot = true;
        }
        else
        {
            textPlaceholder2.text = "";
            imagePlaceholder2.sprite = null;
            if (useFirstSlot)
            {
                useFirstSlot = false;
            }
        }
    }

    private void clearPlaceholders()
    {
        textPlaceholder1.text = "";
        imagePlaceholder1.sprite = null;
        textPlaceholder2.text = "";
        imagePlaceholder2.sprite = null;
        useFirstSlot = true;
    }

    public void ChangeViewDirection(ViewDirection direction)
    {
        if (currentDir == direction)
            return;
        
        clearPlaceholders();
        foreach (Transform child in imagesHolder.transform)
        {
            child.GetComponent<Image_Button>().ChangeActiveImage(direction);
        }
        currentDir = direction;
    }

    public void SetImageFullscreen(Image image)
    {
        fullscreenPlaceholder.GetComponent<Image>().sprite = image.sprite;
        fullscreenPlaceholder.SetActive(true);
    }

    public void hideFullscreen()
    {
        fullscreenPlaceholder.GetComponent<Image>().sprite = null;
        fullscreenPlaceholder.SetActive(false);
    }

    public void PrepareOpen()
    {
        fullscreenPlaceholder.SetActive(false);
        clearPlaceholders();
        ChangeViewDirection(ViewDirection.F);
    }
}
