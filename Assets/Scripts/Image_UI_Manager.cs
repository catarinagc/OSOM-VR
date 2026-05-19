using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Collections;
public class Image_UI_Manager : MonoBehaviour
{
    [SerializeField] TMP_Text image_title;
    [SerializeField] GameObject imagesHolder;
    [SerializeField] GameObject fullscreenPlaceholder;
    [SerializeField] Image fullscreenPlaceholderImage;
    [SerializeField] GameObject imageScreen;
    [SerializeField] UI_Manager UI_Manager;
    [SerializeField] Sprite UIMask;
    public Animator panelAnimator;

    [Header("Placeholder 1")]
    [SerializeField] Image imagePlaceholder1;
    [SerializeField] Image imagePlaceholder1_Shadow;
    [SerializeField] TMP_Text textPlaceholder1;
    [SerializeField] GameObject imageIconPlaceholder1;

    [Header("Placeholder 2")]
    [SerializeField] Image imagePlaceholder2;
    [SerializeField] Image imagePlaceholder2_Shadow;
    [SerializeField] TMP_Text textPlaceholder2;
    [SerializeField] GameObject imageIconPlaceholder2;
    
    [Header("VR only")]
    [SerializeField] GameObject imageOutside;
    [SerializeField] GameObject Controller_UI_Prefab;
    private GameObject InstancedObj;
    public Transform rightController;
    public Transform leftController;

    private bool isDragging = false;
    private InputAction aButton;
    private int hotspotID = 0;
    private char troco_ID = ' ';
    private bool isVR = false;

    //void Awake()
    //{
    //    XRModeSwitcher.OnModeSelected += OnModeChosen;
    //}

    //void OnDestroy()
    //{
    //    XRModeSwitcher.OnModeSelected -= OnModeChosen;
    //}

    public void OnModeChosen(bool isVR)
    {
        Debug.Log("VR" + isVR);
        this.isVR = isVR;
    }

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
        XRModeSwitcher.OnModeSelected += OnModeChosen;
        currentDir = ViewDirection.F;
    }


    public void ShowItem(Sprite newSprite, string year)
    {
        if (textPlaceholder1.text == year || textPlaceholder2.text == year)
            return;

        if (useFirstSlot)
        {
            imagePlaceholder1.sprite = newSprite;
            //imagePlaceholder1.gameObject.SetActive(true);
            //imagePlaceholder1_Shadow.gameObject.SetActive(false);
            textPlaceholder1.text = year;
            imageIconPlaceholder1.SetActive(true);
        }
        else
        {
            imagePlaceholder2.sprite = newSprite;
            //imagePlaceholder2.gameObject.SetActive(true);
            //imagePlaceholder2_Shadow.gameObject.SetActive(false);
            textPlaceholder2.text = year;
            imageIconPlaceholder2.SetActive(true);
        }

        useFirstSlot = !useFirstSlot;
    }

    public void HideItem(bool isFirstSlot)
    {
        if (isFirstSlot)
        {
            textPlaceholder1.text = "";
            //imagePlaceholder1.sprite = null;
            //imagePlaceholder1.gameObject.SetActive(false);
            //imagePlaceholder1_Shadow.gameObject.SetActive(true);
            imagePlaceholder1.sprite = UIMask;
            imageIconPlaceholder1.SetActive(false);
            useFirstSlot = true;
        }
        else
        {
            textPlaceholder2.text = "";
            //imagePlaceholder2.sprite = null;
            //imagePlaceholder2.gameObject.SetActive(false);
            imagePlaceholder2.sprite = UIMask;
            //imagePlaceholder2_Shadow.gameObject.SetActive(true);
            imageIconPlaceholder2.SetActive(false);
            if (useFirstSlot)
            {
                useFirstSlot = false;
            }
        }
    }

    private void clearPlaceholders()
    {
        if (!isVR)
        {
            textPlaceholder1.text = "";
            textPlaceholder2.text = "";
            imagePlaceholder1.sprite = UIMask;
            imagePlaceholder2.sprite = UIMask;
            imageIconPlaceholder1.SetActive(false);
            imageIconPlaceholder2.SetActive(false);
            useFirstSlot = true;
        }
        // hotspotID = 0;
        // troco_ID = ' ';
    }

    private void ResetHotspotData()
    {
        hotspotID = 0;
        troco_ID = ' ';
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
        Image childImage = fullscreenPlaceholderImage;
        childImage.sprite = image.sprite;
        fullscreenPlaceholder.SetActive(true);
    }

    public void hideFullscreen()
    {
        Image childImage = fullscreenPlaceholderImage;
        childImage.sprite = null;
        fullscreenPlaceholder.SetActive(false);
        childImage.GetComponent<UIZoomImage>().OnCloseImage();
    }

    public void PrepareOpen(int hotspotID, char troco_ID)
    {
        clearPlaceholders();
        this.hotspotID = hotspotID;
        this.troco_ID = troco_ID;
        if (!isVR)
            fullscreenPlaceholder.SetActive(false);
        ChangeViewDirection(ViewDirection.F);
        openImages();
    }

    public void openImages()
    {
        image_title.text = "Portimão Poente - Troço " +troco_ID+ " - Ponto " + hotspotID.ToString();
        imageScreen.SetActive(true);
        if (isVR)
        {
            // if (InstancedObj != null)
            // {
            //     Destroy(InstancedObj);
            // }
            InstancedObj = Instantiate(Controller_UI_Prefab);
            //default 3, pode depois mudar conforme hotspot
            InstancedObj.GetComponent<RadialSelection>().numberOfradialPart = 3;
            //
            InstancedObj.transform.SetParent(leftController, false);
            InstancedObj.GetComponent<RadialSelection>().handTransform = rightController;
            InstancedObj.GetComponent<RadialSelection>().image_UI_Manager = this;
            InstancedObj.GetComponent<RadialSelection>().SpawnRadialPart();
        }
        panelAnimator.SetTrigger("Open");
    }
    // public void openImages()
    // {
    //     image_title.text =
    //         "Portimão Poente - Troço " + troco_ID +
    //         " - Ponto " + hotspotID.ToString();

    //     imageScreen.SetActive(true);

    //     if (isVR)
    //     {
    //         if (InstancedObj != null)
    //             Destroy(InstancedObj);

    //         InstancedObj = Instantiate(Controller_UI_Prefab);

    //         RadialSelection radial =
    //             InstancedObj.GetComponent<RadialSelection>();

    //         radial.numberOfradialPart = 3;

    //         InstancedObj.transform.SetParent(leftController, false);

    //         radial.handTransform = rightController;
    //         radial.image_UI_Manager = this;

    //         StartCoroutine(SpawnRadialNextFrame(radial));
    //     }

    //     panelAnimator.SetTrigger("Open");
    // }

    // IEnumerator SpawnRadialNextFrame(RadialSelection radial)
    // {
    //     yield return null;

    //     Canvas.ForceUpdateCanvases();

    //     radial.SpawnRadialPart();
    // }

    public void VR_Arrastar(Image image, string year)
    {
        InstancedObj = Instantiate(imageOutside);

        // InstancedObj.transform.position =
        //     rightController.position + rightController.forward;
        // InstancedObj.transform.position =
        //     rightController.position * 0.3f + rightController.forward;
        Vector3 spawnPos =
            rightController.position +
            rightController.forward;

        spawnPos.y -= 0.2f; // adjust up/down manually
        InstancedObj.transform.position = spawnPos;
        
        Transform target = InstancedObj.transform.Find("Spatial Panel Scroll/Content/OSOMImage");
        Image img = target.GetComponent<Image>();
        img.sprite = image.sprite;
        InstancedObj.GetComponentInChildren<TextMeshProUGUI>().text = year + " - Troço " +troco_ID+ " - Direção " + currentDir + " - Ponto " + hotspotID.ToString();
        
        //Increase sharpness distance (lower mip bias = sharper farther away)
        if (img.sprite != null && img.sprite.texture != null)
        {
            img.sprite.texture.mipMapBias = -1f; // try -0.5 to -2 depending on feel
        }
        UI_Manager.CloseActiveUIs();
    }

    public void imageInteract(Image image, string year)
    {
        if (isVR)
            VR_Arrastar(image, year);
        else
            ShowItem(image.sprite, year);
    }
}
