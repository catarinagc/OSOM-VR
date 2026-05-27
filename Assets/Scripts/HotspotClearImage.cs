using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class HotspotClearImage : MonoBehaviour
{
    [SerializeField] Image_UI_Manager UIManager;
    [SerializeField] ImageAnnotationManager annotationManager;
    [SerializeField] RectTransform imageRect;
    [SerializeField] NoteMarker noteMarkerPrefab;
    [SerializeField] TMP_InputField noteInputField;
    public InspectionImage imageData;
    public int currentYear;
    public int currentHotspotId;
    public string currentDirection;
    public bool isFirstSlot;

    public void OnClickFullscreen()
    {
        UIManager.SetImageFullscreen(imageData);
    }
}
