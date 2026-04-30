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
    [Range(2,10)]
    public int numberOfradialPart;
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public float angleBetweenPart = 10;
    private List<GameObject> spawnedParts = new List<GameObject>();
    public Transform handTransform;
    private int currentSelectedRadialPart = -1;
    [SerializeField] InputActionAsset inputActions;
    private InputAction triggerButton;

    [Range(0f, 1f)]
    public float selectionThreshold = 0.5f; // adjust in inspector
    public float maxSelectDistance = 10f; // 50 cm, adjust as needed
    public UnityEvent<int> OnPartSelected;
    private string[] view_directions = { "F", "T", "L" };
    public Image_UI_Manager image_UI_Manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        triggerButton = inputActions.FindActionMap("XRI Right Interaction").FindAction("trigger");
        triggerButton.Enable();
        SpawnRadialPart();
    }

    // Update is called once per frame
    void Update()
    {
        //GetSelectedRadialPart();
        if (triggerButton.WasPressedThisFrame())
        {
            //Debug.Log(spawnedParts[currentSelectedRadialPart].GetComponentInChildren<TMP_Text>().text);
            string str = spawnedParts[currentSelectedRadialPart].GetComponentInChildren<TMP_Text>().text;
            if (System.Enum.TryParse(str, out Image_UI_Manager.ViewDirection result))
            {
                image_UI_Manager.ChangeViewDirection(result);
            }
            else
            {
                Debug.LogWarning("Unknown view direction string: " + str);
            }
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

            // Calculate angle from center
            float angle = Vector3.SignedAngle(radialPartCanvas.up, localPoint, -radialPartCanvas.forward);

            float distanceFromCenter = localPoint.magnitude;

            if (distanceFromCenter <= maxSelectDistance)
            {
                if (angle < 0) angle += 360;
                currentSelectedRadialPart = (int)angle * numberOfradialPart / 360;
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

    void UpdateVisuals()
    {
        for (int i = 0; i < spawnedParts.Count; i++)
        {
            if (i == currentSelectedRadialPart)
            {
                spawnedParts[i].GetComponent<Image>().color = Color.yellow;
                spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
            }
            else
            {
                spawnedParts[i].GetComponent<Image>().color = Color.white;
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
            float angle = - i * 360 / numberOfradialPart - angleBetweenPart / 2;
            Vector3 radialPartEulerAngle = new Vector3(0, 0, angle);

            GameObject spawnRadialPart = Instantiate(radialPartPrefab, radialPartCanvas);

            spawnRadialPart.transform.position = radialPartCanvas.position;
            spawnRadialPart.transform.localEulerAngles = radialPartEulerAngle;
            // Make text upright
            var tmpText = spawnRadialPart.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
            {
                Transform t = tmpText.transform;

                // Align text "up" with controller's up
                t.rotation = Quaternion.LookRotation(radialPartCanvas.forward, radialPartCanvas.up);
            }
            spawnRadialPart.GetComponent<Image>().fillAmount = 1 / (float)numberOfradialPart - (angleBetweenPart/360);
            if(i < view_directions.Length)
                spawnRadialPart.GetComponentInChildren<TMP_Text>().text = view_directions[i];
            else
                spawnRadialPart.GetComponentInChildren<TMP_Text>().text = "L " + (i - view_directions.Length +1).ToString();
                
            spawnedParts.Add(spawnRadialPart);
        }
    }

    void ClearSelectionVisuals()
    {
        foreach (var part in spawnedParts)
        {
            part.GetComponent<Image>().color = Color.white;
            part.transform.localScale = Vector3.one;
        }
    }
}
