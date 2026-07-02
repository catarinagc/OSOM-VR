using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.InputSystem;
using TMPro;

public class RadialSelection : MonoBehaviour
{
    [Range(2, 10)]
    public int numberOfradialPart;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;
    private List<GameObject> spawnedParts = new List<GameObject>();
    public Transform handTransform;
    private int currentSelectedRadialPart = -1;
    private int confirmedRadialPart = -1;   
    [SerializeField] InputActionAsset inputActions;
    private InputAction triggerButton;

    [Range(0f, 1f)]
    public float selectionThreshold = 0.5f; // adjust in inspector
    public float maxSelectDistance = 10f; // 50 cm, adjust as needed
    public UnityEvent<int> OnPartSelected;
    private string[] view_directions = { "F", "T", "L" };
    public Image_UI_Manager image_UI_Manager;

    void Start()
    {
        triggerButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("trigger");
        triggerButton.Enable();
    }

    // void Update()
    // {
    //     if (triggerButton.WasPressedThisFrame())
    //     {
    //         if (currentSelectedRadialPart < 0 ||
    //             currentSelectedRadialPart >= spawnedParts.Count)
    //         {
    //             return;
    //         }

    //         string dir = spawnedParts[currentSelectedRadialPart]
    //             .GetComponentInChildren<TMP_Text>().text;

    //         TelemetryLogger.Instance.LogUIInteraction("Change Image Direction");
            
    //         image_UI_Manager.ShowDirection(dir);

    //     }
    //     UpdatePointerSelection();
    // }

    void Update()
    {
        if (triggerButton.WasPressedThisFrame())
        {
            if (currentSelectedRadialPart < 0 ||
                currentSelectedRadialPart >= spawnedParts.Count)
            {
                return;
            }

            confirmedRadialPart = currentSelectedRadialPart;

            string dir = spawnedParts[confirmedRadialPart]
                .GetComponentInChildren<TMP_Text>().text;

            TelemetryLogger.Instance.LogUIInteraction("Change Image Direction");

            image_UI_Manager.ShowDirection(dir);

            UpdateVisuals();
        }
        UpdatePointerSelection();
    }

    public void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    void UpdatePointerSelection()
    {
        Ray ray = new Ray(handTransform.position, handTransform.forward);
        Plane menuPlane = new Plane(radialPartCanvas.forward, radialPartCanvas.position);

        if (menuPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 localPoint = radialPartCanvas.InverseTransformPoint(hitPoint); // relative to canvas

            float angle = Mathf.Atan2(localPoint.x, localPoint.y) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360;

            float distanceFromCenter = localPoint.magnitude;

            currentSelectedRadialPart = (int)(angle * numberOfradialPart / 360f);

            if (distanceFromCenter <= maxSelectDistance)
            {
                if (angle < 0) angle += 360;
                currentSelectedRadialPart = (int)(angle * numberOfradialPart / 360f);
            }
            else
            {
                currentSelectedRadialPart = -1;
            }

            UpdateVisuals();
        }
        else
        {
            currentSelectedRadialPart = -1;
            UpdateVisuals();
        }
    }

    // void UpdateVisuals()
    // {
    //     for (int i = 0; i < spawnedParts.Count; i++)
    //     {
    //         if (i == currentSelectedRadialPart)
    //         {
    //             spawnedParts[i].GetComponent<Image>().color = Color.blue;
    //             spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
    //         }
    //         else
    //         {
    //             spawnedParts[i].GetComponent<Image>().color = Color.black;
    //             spawnedParts[i].transform.localScale = Vector3.one;
    //         }
    //     }
    // }

    void UpdateVisuals()
    {
        for (int i = 0; i < spawnedParts.Count; i++)
        {
            bool isConfirmed = (i == confirmedRadialPart);
            bool isHovered = (i == currentSelectedRadialPart);

            var img = spawnedParts[i].GetComponent<Image>();

            if (isConfirmed)
            {
                // Selected: always blue, normal size, ignore hover growth
                img.color = Color.blue;
                spawnedParts[i].transform.localScale = Vector3.one;
            }
            else if (isHovered)
            {
                // Hovered but not selected: black, slightly bigger
                img.color = Color.black;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                // Neither: default
                img.color = Color.black;
                spawnedParts[i].transform.localScale = Vector3.one;
            }
        }
    }
    
    public void SpawnRadialPart()
    {
        radialPartCanvas.gameObject.SetActive(true);

        foreach (var item in spawnedParts)
        {
            Destroy(item);
        }

        spawnedParts.Clear();

        for (int i = 0; i < numberOfradialPart; i++)
        {
            float fillAmount = 1f / numberOfradialPart - (angleBetweenPart / 360f);
            float startAngle = -i * 360f / numberOfradialPart - angleBetweenPart / 2f;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, startAngle);

            GameObject spawnRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);
            spawnRadialPart.transform.position = radialPartCanvas.position;
            spawnRadialPart.transform.localEulerAngles = radialPartEulerAngle;
            spawnRadialPart.GetComponent<Image>().fillAmount = fillAmount;

            var tmpText = spawnRadialPart.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
            {
                if (i < view_directions.Length)
                    tmpText.text = view_directions[i].ToString();
                else
                    tmpText.text = "L " + (i - view_directions.Length + 1).ToString();

                // 1. Calculate the middle angle relative only to this slice rotation
                float sliceArcDegrees = fillAmount * 360f;
                float localCenterAngleDeg = -(sliceArcDegrees / 2f) + 90f; 
                float localCenterAngleRad = localCenterAngleDeg * Mathf.Deg2Rad;

                // 2. Set distance radius
                float textRadiusOffset = 85f; 

                // 3. Position the text locally relative to the spawned slice parent
                tmpText.transform.localPosition = new Vector3(
                    Mathf.Cos(localCenterAngleRad) * textRadiusOffset,
                    Mathf.Sin(localCenterAngleRad) * textRadiusOffset,
                    0f
                );

                // 4. Force text to stay upright globally
                tmpText.transform.rotation = Quaternion.LookRotation(radialPartCanvas.forward, radialPartCanvas.up);
            }

            spawnedParts.Add(spawnRadialPart);

            int defaultIndex = System.Array.IndexOf(view_directions, "T");
            confirmedRadialPart = (defaultIndex >= 0 && defaultIndex < spawnedParts.Count)
                ? defaultIndex
                : 0;
            currentSelectedRadialPart = confirmedRadialPart; // hover starts on the same slice too

            UpdateVisuals();
        }
    }

    void ClearSelectionVisuals()
    {
        foreach (var part in spawnedParts)
        {
            part.GetComponent<Image>().color = Color.black;
            part.transform.localScale = Vector3.one;
        }
    }
}
