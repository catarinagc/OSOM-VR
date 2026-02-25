using UnityEngine;
using UnityEngine.UI;

public class Image_UI_Manager : MonoBehaviour
{
    [SerializeField] Image imagePlaceholder1;
    [SerializeField] Image imagePlaceholder2;

    private bool useFirstSlot = true;

    public void ShowItem(Sprite newSprite)
    {
        if (useFirstSlot)
        {
            imagePlaceholder1.sprite = newSprite;
        }
        else
        {
            imagePlaceholder2.sprite = newSprite;
        }

        useFirstSlot = !useFirstSlot;
    }

    public void hideItem()
    {
        
    }
}
