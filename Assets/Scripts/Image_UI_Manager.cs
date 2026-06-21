// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
// using UnityEngine.InputSystem;
// using UnityEngine.XR;
// using UnityEngine.XR.Management;
// using System.Collections;
// using System.Collections.Generic;
// public class Image_UI_Manager : MonoBehaviour
// {
//     [SerializeField] TMP_Text image_title;
//     [SerializeField] GameObject imagesHolder;
//     [SerializeField] GameObject fullscreenPlaceholder;
//     [SerializeField] TMP_Text fullscreenPlaceholderText;
//     [SerializeField] Image fullscreenPlaceholderImage;
//     [SerializeField] GameObject imageScreen;
//     public UI_Manager UI_Manager;
//     [SerializeField] Sprite UIMask;
//     public Animator panelAnimator;
//     [SerializeField] GameObject buttonsHolder;
//     [SerializeField] GameObject notePanelPC;

//     [Header("Placeholder 1")]
//     [SerializeField] Image imagePlaceholder1;
//     [SerializeField] Image imagePlaceholder1_Shadow;
//     [SerializeField] TMP_Text textPlaceholder1;
//     [SerializeField] GameObject imageIconPlaceholder1;
//     [SerializeField] GameObject closeButtonPlaceholder1;
//     [SerializeField] GameObject placeholder1;
//     public Image interactedImage1;

//     [Header("Placeholder 2")]
//     [SerializeField] Image imagePlaceholder2;
//     [SerializeField] Image imagePlaceholder2_Shadow;
//     [SerializeField] TMP_Text textPlaceholder2;
//     [SerializeField] GameObject imageIconPlaceholder2;
//     [SerializeField] GameObject closeButtonPlaceholder2;
//     [SerializeField] GameObject placeholder2;
//     public Image interactedImage2;
    
//     [Header("VR only")]
//     [SerializeField] GameObject imageOutside;
//     [SerializeField] GameObject Controller_UI_Prefab;
//     [SerializeField] Transform imageContainer;
//     private GameObject InstancedObj;
//     public Transform rightController;
//     public Transform leftController;
//     [SerializeField] private InputActionReference leftGripAction;
//     [SerializeField] private InputActionReference rightGripAction;
//     [SerializeField] private GameObject imagePrefab;
//     [SerializeField] ImageAnnotationManager annotationManager;
//     [SerializeField] TMP_InputField noteInputField;
//     [SerializeField] GameObject notePanelVR;
//     [SerializeField] GameObject dualImageZoom;
//     [SerializeField] SyncZoomVR_Manager syncZoomVRManager;
    
//     [Header("Interactors")]
//     [SerializeField] public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor leftInteractor;
//     [SerializeField] public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor rightInteractor;
//     private Dictionary<string, List<GameObject>> imagesByDirection = new();
//     private List<InspectionImage> currentImages;
//     private bool isDragging = false;
//     private InputAction aButton;
//     private int hotspotID = 0;
//     private char troco_ID = ' ';
//     private bool isVR = false;
//     private string currentDir = "";
//     private bool useFirstSlot = true;

//     public void OnModeChosen(bool isVR)
//     {
//         Debug.Log("VR" + isVR);
//         this.isVR = isVR;
//     }


//     void Start()
//     {
//         XRModeSwitcher.OnModeSelected += OnModeChosen;
//     }

//     public void ShowItem(Image image, string year, InspectionImage imageData)
//     {
//         if (textPlaceholder1.text == year || textPlaceholder2.text == year)
//             return;

//         if (useFirstSlot)
//         {
//             //antiga imagem
//             if (interactedImage1)
//             {
//                 interactedImage1.color = new Color(1f, 1f, 1f, 1f);
//                 placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
//                 interactedImage1.transform.parent.GetComponent<ButtonTooltip>().isActive = true;
//             }
//             //
//             imagePlaceholder1.sprite = image.sprite;
//             imagePlaceholder1.color = new Color(1f, 1f, 1f, 1f);
//             textPlaceholder1.text = year;
//             interactedImage1 = image;
//             interactedImage1.color = new Color(1f, 1f, 1f, 0.47f);
//             interactedImage1.transform.parent.GetComponent<ButtonTooltip>().isActive = false;
//             placeholder1.GetComponent<ImageDisplayController>().currentYear = int.Parse(imageData.year);
//             placeholder1.GetComponent<ImageDisplayController>().currentHotspotId = imageData.hotspotID;
//             placeholder1.GetComponent<ImageDisplayController>().currentDirection = imageData.dir;
//             placeholder1.GetComponent<HotspotClearImage>().imageData = imageData;
//             placeholder1.GetComponent<ImageDisplayController>().SpawnMarkers();
//             imageIconPlaceholder1.SetActive(true);
//             closeButtonPlaceholder1.SetActive(true);
//         }
//         else
//         {
//             if (interactedImage2)
//             {
//                 interactedImage2.color = new Color(1f, 1f, 1f, 1f);
//                 placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
//                 interactedImage2.transform.parent.GetComponent<ButtonTooltip>().isActive = true;
//             }
//             imagePlaceholder2.sprite = image.sprite;
//             imagePlaceholder2.color = new Color(1f, 1f, 1f, 1f);
//             textPlaceholder2.text = year;
//             interactedImage2 = image;
//             interactedImage2.color = new Color(1f, 1f, 1f, 0.47f);
//             interactedImage2.transform.parent.GetComponent<ButtonTooltip>().isActive = false;
//             placeholder2.GetComponent<ImageDisplayController>().currentYear = int.Parse(imageData.year);
//             placeholder2.GetComponent<ImageDisplayController>().currentHotspotId = imageData.hotspotID;
//             placeholder2.GetComponent<ImageDisplayController>().currentDirection = imageData.dir;
//             placeholder2.GetComponent<HotspotClearImage>().imageData = imageData;
//             placeholder2.GetComponent<ImageDisplayController>().SpawnMarkers();
//             imageIconPlaceholder2.SetActive(true);
//             closeButtonPlaceholder2.SetActive(true);
//         }

//         useFirstSlot = !useFirstSlot;
//     }

//     public void HideItem(bool isFirstSlot)
//     {
//         if (isFirstSlot)
//         {
//             textPlaceholder1.text = "";
//             imagePlaceholder1.sprite = UIMask;
//             imagePlaceholder1.color = new Color(1f, 1f, 1f, 0.47f);
//             interactedImage1.color = new Color(1f, 1f, 1f, 1f);
//             interactedImage1.transform.parent.GetComponent<ButtonTooltip>().isActive = true;
//             interactedImage1 = null;
//             imageIconPlaceholder1.SetActive(false);
//             closeButtonPlaceholder1.SetActive(false);
//             useFirstSlot = true;
//             placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
//         }
//         else
//         {
//             textPlaceholder2.text = "";
//             imagePlaceholder2.sprite = UIMask;
//             imagePlaceholder2.color = new Color(1f, 1f, 1f, 0.47f);
//             interactedImage2.color = new Color(1f, 1f, 1f, 1f);
//             interactedImage2.transform.parent.GetComponent<ButtonTooltip>().isActive = true;
//             interactedImage2 = null;
//             imageIconPlaceholder2.SetActive(false);
//             closeButtonPlaceholder2.SetActive(false);
//             placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
//             if (useFirstSlot)
//                 useFirstSlot = false;
//         }
//     }

//     private void clearPlaceholders()
//     {
//         if (!isVR)
//         {
//             textPlaceholder1.text = "";
//             textPlaceholder2.text = "";
//             imagePlaceholder1.sprite = UIMask;
//             imagePlaceholder2.sprite = UIMask;
//             imageIconPlaceholder1.SetActive(false);
//             closeButtonPlaceholder1.SetActive(false);
//             imageIconPlaceholder2.SetActive(false);
//             closeButtonPlaceholder2.SetActive(false);
//             useFirstSlot = true;
//         }
//     }

//     private void ResetHotspotData()
//     {
//         hotspotID = 0;
//         troco_ID = ' ';
//         currentImages = null;
//     }

//     public void ShowDirection(string dir)
//     {
//         if (currentDir == dir)
//             return;
        
//         clearPlaceholders();

//         if (currentDir != "")
//         {
//             // Hide current
//             if (imagesByDirection.ContainsKey(currentDir))
//                 imagesByDirection[currentDir].ForEach(o => o.SetActive(false)); 
//         }

//         // Show new
//         currentDir = dir;
//         if (imagesByDirection.ContainsKey(dir))
//             imagesByDirection[dir].ForEach(o => o.SetActive(true));
//     }

//     public void SetImageFullscreen(InspectionImage imageData)
//     {
//         Image childImage = fullscreenPlaceholderImage;
//         childImage.sprite = imageData.sprite;
//         fullscreenPlaceholderText.text = imageData.year;
//         fullscreenPlaceholder.GetComponent<ImageDisplayController>().ClearNotes();
//         fullscreenPlaceholder.GetComponent<ImageDisplayController>().currentYear = int.Parse(imageData.year);
//         fullscreenPlaceholder.GetComponent<ImageDisplayController>().currentHotspotId = imageData.hotspotID;
//         fullscreenPlaceholder.GetComponent<ImageDisplayController>().currentDirection = imageData.dir;
//         fullscreenPlaceholder.GetComponent<ImageDisplayController>().panelPC = notePanelPC;
//         fullscreenPlaceholder.GetComponent<ImageDisplayController>().SpawnMarkers();
//         fullscreenPlaceholder.SetActive(true);
//     }

//     public void SetMultiImageFullscreen()
//     {
//         if (interactedImage1 != null && interactedImage2 != null)
//         {
//             DualImageZoom dualZoom = dualImageZoom.GetComponent<DualImageZoom>();
//             dualZoom.left.sprite = imagePlaceholder1.sprite;
//             dualZoom.right.sprite = imagePlaceholder2.sprite;

            
//             // Set up left controller
//             ImageDisplayController leftController = dualZoom.leftController;
//             InspectionImage imageData = placeholder1.GetComponent<HotspotClearImage>().imageData;
//             leftController.ClearNotes();
//             leftController.currentYear = int.Parse(imageData.year);
//             leftController.currentHotspotId = imageData.hotspotID;
//             leftController.currentDirection = imageData.dir;
//             leftController.panelPC = notePanelPC;
//             leftController.SpawnMarkers();

//             // Set up right controller
//             ImageDisplayController rightController = dualZoom.rightController;
//             imageData = placeholder2.GetComponent<HotspotClearImage>().imageData;
//             rightController.ClearNotes();
//             rightController.currentYear = int.Parse(imageData.year);
//             rightController.currentHotspotId = imageData.hotspotID;
//             rightController.currentDirection = imageData.dir;
//             rightController.panelPC = notePanelPC;
//             rightController.SpawnMarkers();

//             dualZoom.currentYears.text = leftController.currentYear.ToString() + " | " + rightController.currentYear.ToString();
//             dualImageZoom.SetActive(true);
//         }
//     }

//     public void hideFullscreen()
//     {
//         Image childImage = fullscreenPlaceholderImage;
//         childImage.sprite = null;
//         fullscreenPlaceholder.SetActive(false);
//         childImage.GetComponent<UIZoomImage>().OnCloseImage();
//         if (interactedImage1)
//         {
//             placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
//             placeholder1.GetComponent<ImageDisplayController>().SpawnMarkers();
//         }

//         if (interactedImage2)
//         {
//             placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
//             placeholder2.GetComponent<ImageDisplayController>().SpawnMarkers();
//         }
//     }

//     public void hideMultiFullscreen()
//     {
//         // Image childImage = fullscreenPlaceholderImage;
//         // childImage.sprite = null;
//         dualImageZoom.SetActive(false);
//         dualImageZoom.GetComponent<DualImageZoom>().OnCloseImage();
//         //childImage.GetComponent<UIZoomImage>().OnCloseImage();
//         if (interactedImage1)
//         {
//             placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
//             placeholder1.GetComponent<ImageDisplayController>().SpawnMarkers();
//         }

//         if (interactedImage2)
//         {
//             placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
//             placeholder2.GetComponent<ImageDisplayController>().SpawnMarkers();
//         }
//     }

//     public void PrepareOpen(int hotspotID, char troco_ID, List<InspectionImage> images)
//     {
//         ResetHotspotData();
//         clearPlaceholders();
//         this.hotspotID = hotspotID;
//         this.troco_ID = troco_ID;

//         foreach (InspectionImage image in images)
//         {
//             if (!imagesByDirection.ContainsKey(image.dir))
//                 imagesByDirection[image.dir] = new List<GameObject>();

//             GameObject obj = Instantiate(imagePrefab, imageContainer);
//             obj.GetComponentInChildren<TMP_Text>().text = image.year;
//             obj.GetComponentInChildren<Image>().sprite = image.sprite;
//             obj.GetComponent<Image_Button>().UIManager = this;
//             obj.GetComponent<Image_Button>().imageData = image;
//             obj.SetActive(false);
//             imagesByDirection[image.dir].Add(obj);
//         }

//         if (!isVR)
//         {
//             fullscreenPlaceholder.SetActive(false);
//             PrepareButtons();
//         }

//         ShowDirection("F");
//         openImages();
//     }

//     private void PrepareButtons()
//     {
//         foreach (Transform child in buttonsHolder.transform)
//         {
//             TMP_Text label = child.GetComponentInChildren<TMP_Text>();
//             if (label == null) continue;

//             bool hasImages = imagesByDirection.ContainsKey(label.text);
//             child.gameObject.SetActive(hasImages);
//         }
//     }

//     public void openImages()
//     {
//         image_title.text = "Portimão Poente - Troço " +troco_ID+ " - Ponto " + hotspotID.ToString();
//         imageScreen.SetActive(true);
//         if (isVR)
//         {
//             InstancedObj = Instantiate(Controller_UI_Prefab);
//             //default 3, pode depois mudar conforme hotspot
//             InstancedObj.GetComponent<RadialSelection>().numberOfradialPart = imagesByDirection.Count;
//             InstancedObj.transform.SetParent(leftController, false);
//             InstancedObj.GetComponent<RadialSelection>().handTransform = rightController;
//             InstancedObj.GetComponent<RadialSelection>().image_UI_Manager = this;
//             InstancedObj.GetComponent<RadialSelection>().SpawnRadialPart();
//         }
//         panelAnimator.SetTrigger("Open");
//     }

//     public void VR_Arrastar(Image image, string year, InspectionImage imageData)
//     {
//         InstancedObj = Instantiate(imageOutside);

//         Vector3 spawnPos =
//             rightController.position +
//             rightController.forward;

//         spawnPos.y -= 0.2f; // adjust down
//         InstancedObj.transform.position = spawnPos;
        
//         Transform target = InstancedObj.transform.Find("Spatial Panel Scroll/Content/OSOMImage");
//         Image img = target.GetComponent<Image>();
//         img.sprite = image.sprite;
//         InstancedObj.GetComponentInChildren<TextMeshProUGUI>().text = year + " - Troço " +troco_ID+ " - Direção " + currentDir + " - Ponto " + hotspotID.ToString();
        
//         InstancedObj.GetComponent<ImageDisplayController>().isVR = isVR;
//         InstancedObj.GetComponent<ImageDisplayController>().ClearNotes();
//         InstancedObj.GetComponent<ImageDisplayController>().currentYear = int.Parse(imageData.year);
//         InstancedObj.GetComponent<ImageDisplayController>().currentHotspotId = imageData.hotspotID;
//         InstancedObj.GetComponent<ImageDisplayController>().currentDirection = imageData.dir;
//         InstancedObj.GetComponent<ImageDisplayController>().UIManager = this;
//         InstancedObj.GetComponent<ImageDisplayController>().annotationManager = annotationManager;
//         InstancedObj.GetComponent<ImageDisplayController>().noteInputField = noteInputField;
//         InstancedObj.GetComponent<ImageDisplayController>().imageData = imageData;
//         InstancedObj.GetComponent<ImageDisplayController>().panelVR = notePanelVR;
//         InstancedObj.GetComponent<ImageDisplayController>().SpawnMarkers();
//         InstancedObj.GetComponent<ImageDisplayController>().vrControllerRay = rightController;

//         InstancedObj.GetComponent<ImageDisplayController>().leftGripAction = leftGripAction;
//         InstancedObj.GetComponent<ImageDisplayController>().rightGripAction = rightGripAction;
//         InstancedObj.GetComponent<ImageDisplayController>().leftControllerTransform = leftController;
//         InstancedObj.GetComponent<ImageDisplayController>().rightControllerTransform = rightController;
//         InstancedObj.GetComponent<ImageDisplayController>().leftInteractor = leftInteractor;
//         InstancedObj.GetComponent<ImageDisplayController>().rightInteractor = rightInteractor;
//         InstancedObj.GetComponent<ImageDisplayController>().syncManager = syncZoomVRManager;

//         //Increase sharpness distance (lower mip bias = sharper farther away)
//         if (img.sprite != null && img.sprite.texture != null)
//         {
//             img.sprite.texture.mipMapBias = -1f;
//         }
//         UI_Manager.CloseActiveUIs();
//     }

//     public void imageInteract(Image image, string year, InspectionImage imageData)
//     {
//         if (isVR)
//             VR_Arrastar(image, year, imageData);
//         else
//             ShowItem(image, year, imageData);
//     }

//     public void Close()
//     {
//         foreach (var list in imagesByDirection.Values)
//             list.ForEach(o => Destroy(o));

//         imagesByDirection.Clear();
//         currentDir = "";
//         gameObject.SetActive(false);
//         if (!isVR)
//         {
//             placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
//             placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
//             fullscreenPlaceholder.GetComponent<ImageDisplayController>().ClearNotes(); 
//             dualImageZoom.SetActive(false);       
//         }
//     }
// }


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using System.Collections;
using System.Collections.Generic;
public class Image_UI_Manager : MonoBehaviour
{
    [SerializeField] TMP_Text image_title;
    [SerializeField] GameObject imagesHolder;
    [SerializeField] GameObject fullscreenPlaceholder;
    [SerializeField] TMP_Text fullscreenPlaceholderText;
    [SerializeField] Image fullscreenPlaceholderImage;
    [SerializeField] GameObject imageScreen;
    public UI_Manager UI_Manager;
    [SerializeField] Sprite UIMask;
    public Animator panelAnimator;
    [SerializeField] GameObject buttonsHolder;
    [SerializeField] GameObject notePanelPC;

    [Header("Placeholder 1")]
    [SerializeField] Image imagePlaceholder1;
    [SerializeField] Image imagePlaceholder1_Shadow;
    [SerializeField] TMP_Text textPlaceholder1;
    [SerializeField] GameObject imageIconPlaceholder1;
    [SerializeField] GameObject closeButtonPlaceholder1;
    [SerializeField] GameObject placeholder1;
    public Image interactedImage1;

    [Header("Placeholder 2")]
    [SerializeField] Image imagePlaceholder2;
    [SerializeField] Image imagePlaceholder2_Shadow;
    [SerializeField] TMP_Text textPlaceholder2;
    [SerializeField] GameObject imageIconPlaceholder2;
    [SerializeField] GameObject closeButtonPlaceholder2;
    [SerializeField] GameObject placeholder2;
    public Image interactedImage2;
    
    [Header("VR only")]
    [SerializeField] GameObject imageOutside;
    [SerializeField] GameObject Controller_UI_Prefab;
    [SerializeField] Transform imageContainer;
    private GameObject InstancedObj;
    public Transform rightController;
    public Transform leftController;
    [SerializeField] private InputActionReference leftGripAction;
    [SerializeField] private InputActionReference rightGripAction;
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] ImageAnnotationManager annotationManager;
    [SerializeField] TMP_InputField noteInputField;
    [SerializeField] GameObject notePanelVR;
    [SerializeField] GameObject dualImageZoom;
    [SerializeField] SyncZoomVR_Manager syncZoomVRManager;
    
    [Header("Interactors")]
    [SerializeField] public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor leftInteractor;
    [SerializeField] public UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor rightInteractor;
    private Dictionary<string, List<GameObject>> imagesByDirection = new();
    private List<InspectionImage> currentImages;
    private bool isDragging = false;
    private InputAction aButton;
    private int hotspotID = 0;
    private char troco_ID = ' ';
    private bool isVR = false;
    private string currentDir = "";
    private bool useFirstSlot = true;

    public void OnModeChosen(bool isVR)
    {
        Debug.Log("VR" + isVR);
        this.isVR = isVR;
    }


    void Start()
    {
        XRModeSwitcher.OnModeSelected += OnModeChosen;
    }

    public void ShowItem(Image image, string year, InspectionImage imageData)
    {
        if (textPlaceholder1.text == year || textPlaceholder2.text == year)
            return;

        if (useFirstSlot)
        {
            //antiga imagem
            if (interactedImage1)
            {
                interactedImage1.color = new Color(1f, 1f, 1f, 1f);
                placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
                interactedImage1.transform.parent.GetComponent<ButtonTooltip>().isActive = true;
            }
            //
            imagePlaceholder1.sprite = image.sprite;
            imagePlaceholder1.color = new Color(1f, 1f, 1f, 1f);
            textPlaceholder1.text = year;
            interactedImage1 = image;
            interactedImage1.color = new Color(1f, 1f, 1f, 0.47f);
            interactedImage1.transform.parent.GetComponent<ButtonTooltip>().isActive = false;
            placeholder1.GetComponent<ImageDisplayController>().currentYear = int.Parse(imageData.year);
            placeholder1.GetComponent<ImageDisplayController>().currentHotspotId = imageData.hotspotID;
            placeholder1.GetComponent<ImageDisplayController>().currentDirection = imageData.dir;
            placeholder1.GetComponent<HotspotClearImage>().imageData = imageData;
            placeholder1.GetComponent<ImageDisplayController>().SpawnMarkers();
            imageIconPlaceholder1.SetActive(true);
            closeButtonPlaceholder1.SetActive(true);
        }
        else
        {
            if (interactedImage2)
            {
                interactedImage2.color = new Color(1f, 1f, 1f, 1f);
                placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
                interactedImage2.transform.parent.GetComponent<ButtonTooltip>().isActive = true;
            }
            imagePlaceholder2.sprite = image.sprite;
            imagePlaceholder2.color = new Color(1f, 1f, 1f, 1f);
            textPlaceholder2.text = year;
            interactedImage2 = image;
            interactedImage2.color = new Color(1f, 1f, 1f, 0.47f);
            interactedImage2.transform.parent.GetComponent<ButtonTooltip>().isActive = false;
            placeholder2.GetComponent<ImageDisplayController>().currentYear = int.Parse(imageData.year);
            placeholder2.GetComponent<ImageDisplayController>().currentHotspotId = imageData.hotspotID;
            placeholder2.GetComponent<ImageDisplayController>().currentDirection = imageData.dir;
            placeholder2.GetComponent<HotspotClearImage>().imageData = imageData;
            placeholder2.GetComponent<ImageDisplayController>().SpawnMarkers();
            imageIconPlaceholder2.SetActive(true);
            closeButtonPlaceholder2.SetActive(true);
        }

        useFirstSlot = !useFirstSlot;
    }

    public void HideItem(bool isFirstSlot)
    {
        if (isFirstSlot)
        {
            textPlaceholder1.text = "";
            imagePlaceholder1.sprite = UIMask;
            imagePlaceholder1.color = new Color(1f, 1f, 1f, 0.47f);
            interactedImage1.color = new Color(1f, 1f, 1f, 1f);
            interactedImage1.transform.parent.GetComponent<ButtonTooltip>().isActive = true;
            interactedImage1 = null;
            imageIconPlaceholder1.SetActive(false);
            closeButtonPlaceholder1.SetActive(false);
            useFirstSlot = true;
            placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
        }
        else
        {
            textPlaceholder2.text = "";
            imagePlaceholder2.sprite = UIMask;
            imagePlaceholder2.color = new Color(1f, 1f, 1f, 0.47f);
            interactedImage2.color = new Color(1f, 1f, 1f, 1f);
            interactedImage2.transform.parent.GetComponent<ButtonTooltip>().isActive = true;
            interactedImage2 = null;
            imageIconPlaceholder2.SetActive(false);
            closeButtonPlaceholder2.SetActive(false);
            placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
            if (useFirstSlot)
                useFirstSlot = false;
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
            closeButtonPlaceholder1.SetActive(false);
            imageIconPlaceholder2.SetActive(false);
            closeButtonPlaceholder2.SetActive(false);
            useFirstSlot = true;
        }
    }

    private void ResetHotspotData()
    {
        hotspotID = 0;
        troco_ID = ' ';
        currentImages = null;
    }

    public void ShowDirection(string dir)
    {
        if (currentDir == dir)
            return;
        
        clearPlaceholders();

        if (currentDir != "")
        {
            // Hide current
            if (imagesByDirection.ContainsKey(currentDir))
                imagesByDirection[currentDir].ForEach(o => o.SetActive(false)); 
        }

        // Show new
        currentDir = dir;
        if (imagesByDirection.ContainsKey(dir))
            imagesByDirection[dir].ForEach(o => o.SetActive(true));
    }

    public void SetImageFullscreen(InspectionImage imageData)
    {
        Image childImage = fullscreenPlaceholderImage;
        childImage.sprite = imageData.sprite;
        fullscreenPlaceholderText.text = imageData.year;
        fullscreenPlaceholder.GetComponent<ImageDisplayController>().ClearNotes();
        fullscreenPlaceholder.GetComponent<ImageDisplayController>().currentYear = int.Parse(imageData.year);
        fullscreenPlaceholder.GetComponent<ImageDisplayController>().currentHotspotId = imageData.hotspotID;
        fullscreenPlaceholder.GetComponent<ImageDisplayController>().currentDirection = imageData.dir;
        fullscreenPlaceholder.GetComponent<ImageDisplayController>().panelPC = notePanelPC;
        fullscreenPlaceholder.GetComponent<ImageDisplayController>().SpawnMarkers();
        fullscreenPlaceholder.SetActive(true);
    }

    public void SetMultiImageFullscreen()
    {
        if (interactedImage1 != null && interactedImage2 != null)
        {
            DualImageZoom dualZoom = dualImageZoom.GetComponent<DualImageZoom>();
            dualZoom.left.sprite = imagePlaceholder1.sprite;
            dualZoom.right.sprite = imagePlaceholder2.sprite;

            
            // Set up left controller
            ImageDisplayController leftController = dualZoom.leftController;
            InspectionImage imageData = placeholder1.GetComponent<HotspotClearImage>().imageData;
            leftController.ClearNotes();
            leftController.currentYear = int.Parse(imageData.year);
            leftController.currentHotspotId = imageData.hotspotID;
            leftController.currentDirection = imageData.dir;
            leftController.panelPC = notePanelPC;
            leftController.SpawnMarkers();

            // Set up right controller
            ImageDisplayController rightController = dualZoom.rightController;
            imageData = placeholder2.GetComponent<HotspotClearImage>().imageData;
            rightController.ClearNotes();
            rightController.currentYear = int.Parse(imageData.year);
            rightController.currentHotspotId = imageData.hotspotID;
            rightController.currentDirection = imageData.dir;
            rightController.panelPC = notePanelPC;
            rightController.SpawnMarkers();

            dualZoom.currentYears.text = leftController.currentYear.ToString() + " | " + rightController.currentYear.ToString();
            dualImageZoom.SetActive(true);
        }
    }

    public void hideFullscreen()
    {
        Image childImage = fullscreenPlaceholderImage;
        childImage.sprite = null;
        fullscreenPlaceholder.SetActive(false);
        childImage.GetComponent<UIZoomImage>().OnCloseImage();
        if (interactedImage1)
        {
            placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
            placeholder1.GetComponent<ImageDisplayController>().SpawnMarkers();
        }

        if (interactedImage2)
        {
            placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
            placeholder2.GetComponent<ImageDisplayController>().SpawnMarkers();
        }
    }

    public void hideMultiFullscreen()
    {
        // Image childImage = fullscreenPlaceholderImage;
        // childImage.sprite = null;
        dualImageZoom.SetActive(false);
        dualImageZoom.GetComponent<DualImageZoom>().OnCloseImage();
        //childImage.GetComponent<UIZoomImage>().OnCloseImage();
        if (interactedImage1)
        {
            placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
            placeholder1.GetComponent<ImageDisplayController>().SpawnMarkers();
        }

        if (interactedImage2)
        {
            placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
            placeholder2.GetComponent<ImageDisplayController>().SpawnMarkers();
        }
    }

    public void PrepareOpen(int hotspotID, char troco_ID, List<InspectionImage> images)
    {
        ResetHotspotData();
        clearPlaceholders();
        this.hotspotID = hotspotID;
        this.troco_ID = troco_ID;

        foreach (InspectionImage image in images)
        {
            if (!imagesByDirection.ContainsKey(image.dir))
                imagesByDirection[image.dir] = new List<GameObject>();

            GameObject obj = Instantiate(imagePrefab, imageContainer);
            obj.GetComponentInChildren<TMP_Text>().text = image.year;
            obj.GetComponentInChildren<Image>().sprite = image.sprite;
            obj.GetComponent<Image_Button>().UIManager = this;
            obj.GetComponent<Image_Button>().imageData = image;
            obj.SetActive(false);
            imagesByDirection[image.dir].Add(obj);
        }

        if (!isVR)
        {
            fullscreenPlaceholder.SetActive(false);
            PrepareButtons();
        }

        ShowDirection("F");
        openImages();
    }

    private void PrepareButtons()
    {
        foreach (Transform child in buttonsHolder.transform)
        {
            TMP_Text label = child.GetComponentInChildren<TMP_Text>();
            if (label == null) continue;

            bool hasImages = imagesByDirection.ContainsKey(label.text);
            child.gameObject.SetActive(hasImages);
        }
    }

    public void openImages()
    {
        image_title.text = "Portimão Poente - Troço " +troco_ID+ " - Ponto " + hotspotID.ToString();
        imageScreen.SetActive(true);
        if (isVR)
        {
            InstancedObj = Instantiate(Controller_UI_Prefab);
            //default 3, pode depois mudar conforme hotspot
            InstancedObj.GetComponent<RadialSelection>().numberOfradialPart = imagesByDirection.Count;
            InstancedObj.transform.SetParent(leftController, false);
            InstancedObj.GetComponent<RadialSelection>().handTransform = rightController;
            InstancedObj.GetComponent<RadialSelection>().image_UI_Manager = this;
            InstancedObj.GetComponent<RadialSelection>().SpawnRadialPart();
        }
        panelAnimator.SetTrigger("Open");
    }

    public void VR_Arrastar(Image image, string year, InspectionImage imageData)
    {
        InstancedObj = Instantiate(imageOutside);

        Vector3 spawnPos =
            rightController.position +
            rightController.forward;

        spawnPos.y -= 0.2f; // adjust down
        InstancedObj.transform.position = spawnPos;
        
        Transform target = InstancedObj.transform.Find("Spatial Panel Scroll/Content/OSOMImage");
        Image img = target.GetComponent<Image>();
        img.sprite = image.sprite;
        InstancedObj.GetComponentInChildren<TextMeshProUGUI>().text = year + " - Troço " +troco_ID+ " - Direção " + currentDir + " - Ponto " + hotspotID.ToString();
        
        InstancedObj.GetComponent<ImageDisplayController>().isVR = isVR;
        InstancedObj.GetComponent<ImageDisplayController>().ClearNotes();
        InstancedObj.GetComponent<ImageDisplayController>().currentYear = int.Parse(imageData.year);
        InstancedObj.GetComponent<ImageDisplayController>().currentHotspotId = imageData.hotspotID;
        InstancedObj.GetComponent<ImageDisplayController>().currentDirection = imageData.dir;
        InstancedObj.GetComponent<ImageDisplayController>().UIManager = this;
        InstancedObj.GetComponent<ImageDisplayController>().annotationManager = annotationManager;
        InstancedObj.GetComponent<ImageDisplayController>().noteInputField = noteInputField;
        InstancedObj.GetComponent<ImageDisplayController>().imageData = imageData;
        InstancedObj.GetComponent<ImageDisplayController>().panelVR = notePanelVR;
        InstancedObj.GetComponent<ImageDisplayController>().SpawnMarkers();
        InstancedObj.GetComponent<ImageDisplayController>().vrControllerRay = rightController;

        InstancedObj.GetComponent<ImageDisplayController>().leftGripAction = leftGripAction;
        InstancedObj.GetComponent<ImageDisplayController>().rightGripAction = rightGripAction;
        InstancedObj.GetComponent<ImageDisplayController>().leftControllerTransform = leftController;
        InstancedObj.GetComponent<ImageDisplayController>().rightControllerTransform = rightController;
        InstancedObj.GetComponent<ImageDisplayController>().leftInteractor = leftInteractor;
        InstancedObj.GetComponent<ImageDisplayController>().rightInteractor = rightInteractor;
        InstancedObj.GetComponent<ImageDisplayController>().syncManager = syncZoomVRManager;

        // This copy is independent of the main gallery panel and can be left
        // open in the world after the panel closes. Mark it so its own
        // OnDestroy() releases this reference, and take out a reference now
        // so HotspotManager keeps this hotspot's sprites alive until then.
        InstancedObj.GetComponent<ImageDisplayController>().isDynamicWorldCopy = true;
        HotspotManager.Instance.AddHotspotReference(imageData.hotspotID);

        //Increase sharpness distance (lower mip bias = sharper farther away)
        if (img.sprite != null && img.sprite.texture != null)
        {
            img.sprite.texture.mipMapBias = -1f;
        }
        UI_Manager.CloseActiveUIs();
    }

    public void imageInteract(Image image, string year, InspectionImage imageData)
    {
        if (isVR)
            VR_Arrastar(image, year, imageData);
        else
            ShowItem(image, year, imageData);
    }

    public void Close()
    {
        foreach (var list in imagesByDirection.Values)
            list.ForEach(o => Destroy(o));

        imagesByDirection.Clear();
        currentDir = "";
        gameObject.SetActive(false);

        // Release the reference taken out when this hotspot's gallery was
        // opened (see HotspotScript.OnInteract -> RequestHotspotImages).
        // Any dragged-out VR copies hold their own separate reference and
        // are unaffected by this.
        if (hotspotID != 0)
            HotspotManager.Instance.ReleaseHotspotReference(hotspotID);

        if (!isVR)
        {
            placeholder1.GetComponent<ImageDisplayController>().ClearNotes();
            placeholder2.GetComponent<ImageDisplayController>().ClearNotes();
            fullscreenPlaceholder.GetComponent<ImageDisplayController>().ClearNotes(); 
            dualImageZoom.SetActive(false);       
        }
    }
}