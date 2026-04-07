using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class Image_UI_Manager : MonoBehaviour
{
    [SerializeField] Image imagePlaceholder1;
    [SerializeField] TMP_Text textPlaceholder1;
    [SerializeField] Image imagePlaceholder2;
    [SerializeField] TMP_Text textPlaceholder2;
    [SerializeField] TMP_Text image_title;
    [SerializeField] TMP_Text init_title;

    [SerializeField] GameObject imagesHolder;
    [SerializeField] GameObject imageOutside;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] GameObject fullscreenPlaceholder;
    [SerializeField] GameObject initScreen;
    [SerializeField] GameObject imageScreen;
    [SerializeField] UI_Manager UI_Manager;
    [SerializeField] GameObject Controller_UI_Prefab;

    private GameObject InstancedObj;
    public Animator panelAnimator;
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
        if (isVR)
        {
            aButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("AButton");
            aButton.Enable();
        }
    }

    //void Update()
    //{
        
    //    if (isDragging)
    //    {
    //        if (aButton.WasPressedThisFrame())
    //        {
    //            isDragging = false;
    //        }
    //        else
    //        {
    //            InstancedObj.transform.position = rightController.position + rightController.forward /* * 0.5f*/;
    //            InstancedObj.transform.rotation = rightController.rotation;
    //        }
    //    }
    //}

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
        Image childImage = fullscreenPlaceholder.GetComponentInChildren<Image>();
        childImage.sprite = image.sprite;
        fullscreenPlaceholder.SetActive(true);
    }

    public void hideFullscreen()
    {
        Image childImage = fullscreenPlaceholder.GetComponentInChildren<Image>();
        childImage.sprite = null;
        fullscreenPlaceholder.SetActive(false);
        childImage.GetComponent<UIZoomImage>().OnCloseImage();
    }

    public void PrepareOpen(int hotspotID, char troco_ID)
    {
        this.hotspotID = hotspotID;
        this.troco_ID = troco_ID;
        init_title.text = "Portimão Poente - Troço " +troco_ID+ " - Ponto " + hotspotID.ToString();
        initScreen.SetActive(true);
        imageScreen.SetActive(false);
        fullscreenPlaceholder.SetActive(false);
        clearPlaceholders();
        ChangeViewDirection(ViewDirection.F);
    }

    public void openImages()
    {
        image_title.text = init_title.text;
        initScreen.SetActive(false);
        imageScreen.SetActive(true);
        if (isVR)
        {
            InstancedObj = Instantiate(Controller_UI_Prefab);
            //default 3, pode depois mudar conforme hotspot
            InstancedObj.GetComponent<RadialSelection>().numberOfradialPart = 3;
            //
            InstancedObj.transform.SetParent(leftController, false);
            InstancedObj.GetComponent<RadialSelection>().handTransform = rightController;
            InstancedObj.GetComponent<RadialSelection>().image_UI_Manager = this;
        }
        panelAnimator.SetTrigger("Open");
    }

    public void VR_Arrastar(Image image, string year)
    {
        InstancedObj = Instantiate(imageOutside);

        InstancedObj.transform.position =
            rightController.position + rightController.forward;
        InstancedObj.transform.rotation = rightController.rotation;

        isDragging = true;

        InstancedObj.GetComponentInChildren<Image>().sprite = image.sprite;
        InstancedObj.GetComponentInChildren<TextMeshProUGUI>().text = year;
        InstancedObj.GetComponent<Movable_UI>().rightController = rightController;
        InstancedObj.GetComponent<Movable_UI>().leftController = leftController;
        InstancedObj.GetComponent<Movable_UI>().inputActions = inputActions;

        UI_Manager.CloseActiveUI();
    }

    public void imageInteract(Image image, string year)
    {
        if (isVR)
            VR_Arrastar(image, year);
        else
            ShowItem(image.sprite, year);
    }
}
