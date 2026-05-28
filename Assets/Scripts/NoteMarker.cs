using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
public class NoteMarker : MonoBehaviour
{
    public NoteData data;
    [SerializeField] GameObject tooltipPanelVR;
    [SerializeField] GameObject tooltipPanelPC;
    private GameObject activePanel;
    [SerializeField] TMP_Text tooltipTextVR;
    [SerializeField] TMP_Text tooltipTextDateVR;
    [SerializeField] TMP_Text tooltipTextPC;
    [SerializeField] TMP_Text tooltipTextDatePC;
    [SerializeField] TMP_Text activeTooltipText;
    [SerializeField] TMP_Text activeTooltipTextDate;
    public ImageDisplayController displayController;
    private bool isVR;
    private bool canInteract;

    public void Initialize(NoteData noteData, RectTransform imageRect, bool isVR, bool canInteract, GameObject panelVR, GameObject panelPC)
    {
        this.canInteract = canInteract;
        data = noteData;
        this.isVR = isVR;
        if (isVR)
        {
            // activePanel = tooltipPanelVR;
            // activeTooltipText = tooltipTextVR;
            // activeTooltipTextDate = tooltipTextDateVR;
            activePanel = panelVR;
        }
        else
        {
            // activePanel = tooltipPanelPC;
            // activeTooltipText = tooltipTextPC;
            // activeTooltipTextDate = tooltipTextDatePC;
            activePanel = panelPC;
        }
        SetPosition(noteData.relativePos, imageRect);
    }

    private void SetPosition(Vector2 relativePos, RectTransform imageRect)
    {
        //Vector2 size = imageRect.rect.size;
        Vector2 size = GetActualImageSize(imageRect);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(
            (relativePos.x - 0.5f) * size.x,
            (relativePos.y - 0.5f) * size.y
        );
    }

    private Vector2 GetActualImageSize(RectTransform imageRect)
    {
        Image img = imageRect.GetComponent<Image>();
        if (img == null || img.sprite == null) return imageRect.rect.size;

        float spriteWidth = img.sprite.rect.width;
        float spriteHeight = img.sprite.rect.height;
        float spriteAspect = spriteWidth / spriteHeight;

        float rectWidth = imageRect.rect.width;
        float rectHeight = imageRect.rect.height;
        float rectAspect = rectWidth / rectHeight;

        if (spriteAspect > rectAspect)
        {
            // Letterboxed top and bottom
            return new Vector2(rectWidth, rectWidth / spriteAspect);
        }
        else
        {
            // Letterboxed left and right
            return new Vector2(rectHeight * spriteAspect, rectHeight);
        }
    }

    public void OnClick()
    {
        if (!canInteract)
            return;
        // if (!isVR)
        // {     
        //     activeTooltipText.text = data.message;
        //     activeTooltipTextDate.text = data.created;
        // }
        // if (isVR)
        // {
        //     activePanel.GetComponent<NotePanel>().Open(data.message, data.created, this);
        // }
        activePanel.GetComponent<NotePanel>().Open(data.message, data.created, this);
        activePanel.SetActive(true);
    }

    public void EditMessage()
    {
        displayController.annotationManager.EditNote(data, displayController, () =>
        {
            activePanel.SetActive(false);
        });
    }

    public void DeleteNote()
    {
        displayController.DeleteNote(data);
        Destroy(gameObject);
    }

    public void DisablePanel()
    {
        activePanel.SetActive(false);
    }
}