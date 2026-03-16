using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
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

    [Range(0f, 1f)]
    public float selectionThreshold = 0.5f; // adjust in inspector

    public UnityEvent<int> OnPartSelected;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnRadialPart();
    }

    // Update is called once per frame
    void Update()
    {
        GetSelectedRadialPart();
    }

    public void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    //from tutorial
    //public void GetSelectedRadialPart()
    //{
    //    Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
    //    Vector3 centerToHandProjected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);

    //    float angle = Vector3.SignedAngle(radialPartCanvas.up, centerToHandProjected, -radialPartCanvas.forward);

    //    if (angle < 0)
    //        angle += 360;

    //    currentSelectedRadialPart = (int)angle * numberOfradialPart / 360;

    //    for (int i = 0; i< spawnedParts.Count; i++)
    //    {
    //        if(i == currentSelectedRadialPart)
    //        {
    //            spawnedParts[i].GetComponent<Image>().color = Color.yellow;
    //            spawnedParts[i].transform.localScale = 1.1f * Vector3.one;
    //        }
    //        else
    //        {
    //            spawnedParts[i].GetComponent<Image>().color = Color.white;
    //            spawnedParts[i].transform.localScale =  Vector3.one;
    //        }
    //    }
    //}

    public void GetSelectedRadialPart()
    {
        Vector3 menuForward = radialPartCanvas.forward;
        Vector3 pointerDirection = handTransform.forward;

        // Check if pointing toward the menu
        float alignment = Vector3.Dot(menuForward, pointerDirection);

        if (alignment > -selectionThreshold) // important: negative because facing opposite direction
        {
            currentSelectedRadialPart = -1;
            ClearSelectionVisuals();
            return;
        }

        // Project pointer onto menu plane
        Vector3 projected = Vector3.ProjectOnPlane(pointerDirection, menuForward).normalized;

        float angle = Vector3.SignedAngle(radialPartCanvas.up, projected, -menuForward);

        if (angle < 0)
            angle += 360;

        currentSelectedRadialPart = Mathf.FloorToInt(angle / (360f / numberOfradialPart));

        UpdateVisuals();
    }

    void ClearSelectionVisuals()
    {
        foreach (var part in spawnedParts)
        {
            part.GetComponent<Image>().color = Color.white;
            part.transform.localScale = Vector3.one;
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
        //radialPartCanvas.position = handTransform.position;
        //radialPartCanvas.rotation = handTransform.rotation;

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

            spawnRadialPart.GetComponent<Image>().fillAmount = 1 / (float)numberOfradialPart - (angleBetweenPart/360);
            spawnedParts.Add(spawnRadialPart);
        }
    }
}
