using UnityEngine;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
public class HotspotManager : MonoBehaviour
{
    [SerializeField] GameObject breakwaterOrigin;
    [SerializeField] float modelScale;
    Vector3 realOriginPos;
    [SerializeField] GameObject hotspotPrefab;
    private List<HotspotScript> hotspots = new List<HotspotScript>();
    //[SerializeField] UI_Manager UI_Manager;
    private UI_Manager view_UI_manager;

    void OnEnable()
    {
        XRModeSwitcher.OnModeSelected += OnModeChosen;
    }

    void OnDisable()
    {
        XRModeSwitcher.OnModeSelected -= OnModeChosen;
    }

    private void OnModeChosen(bool isVR)
    {
        Debug.Log("Mode selected! VR? " + isVR);

        ReadCSV();
    }

    void Start()
    {
        realOriginPos = breakwaterOrigin.GetComponent<originPointScript>().realWorldPosition;
    }

    // void ReadCSV()
    // {
    //     string path = Path.Combine(Application.dataPath, "HotspotData/Hotspot_Data.csv");

    //     if (!File.Exists(path))
    //     {
    //         Debug.LogError("CSV file not found: " + path);
    //         return;
    //     }

    //     string[] lines = File.ReadAllLines(path);

    //     // Start from line index 2
    //     for (int i = 2; i < lines.Length; i++)
    //     {
    //         if (string.IsNullOrWhiteSpace(lines[i]))
    //             continue;

    //         string[] columns = lines[i].Split(',');

    //         string id = columns[0];

    //         float x = float.Parse(columns[1], CultureInfo.InvariantCulture);
    //         float y = float.Parse(columns[2], CultureInfo.InvariantCulture);

    //         //Debug.Log($"ID: {id} | X: {x} | Y: {y}");

    //         GameObject hotspot = CreateHotspot(id, new Vector2(x, y));
    //         ChangeHotspotPosition(hotspot);
    //         AssignZone(hotspot);
    //     }
    // }
    void ReadCSV()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Hotspot_Data.csv");

    #if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(ReadCSVAndroid(path));
    #else
        ReadCSVDesktop(path);
    #endif
    }

    void ReadCSVDesktop(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found: " + path);
            return;
        }

        ProcessCSV(File.ReadAllLines(path));
    }

    IEnumerator ReadCSVAndroid(string path)
    {
        UnityWebRequest req = UnityWebRequest.Get(path);
        yield return req.SendWebRequest();

        string[] lines = req.downloadHandler.text.Split('\n');
        ProcessCSV(lines);
    }

    void ProcessCSV(string[] lines)
    {
        for (int i = 2; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] columns = lines[i].Split(',');

            string id = columns[0];

            float x = float.Parse(columns[1], CultureInfo.InvariantCulture);
            float y = float.Parse(columns[2], CultureInfo.InvariantCulture);

            GameObject hotspot = CreateHotspot(id, new Vector2(x, y));
            ChangeHotspotPosition(hotspot);
            AssignZone(hotspot);
        }
    }

    Vector2 LatLonToMetersSimple(Vector2 latLon, Vector2 originLatLon)
    {
        float latToMeters = 111320f;
        float lonToMeters = 111320f * Mathf.Cos(originLatLon.y * Mathf.Deg2Rad);

        float dLat = latLon.y - originLatLon.y;
        float dLon = latLon.x - originLatLon.x;

        float x = dLon * lonToMeters; // east-west
        float y = dLat * latToMeters; // north-south

        return new Vector2(x, y);
    }

    //original
    // void ChangeHotspotPosition(GameObject hotspot)
    // {
    //     Vector3 realHotspotPos = hotspot.GetComponent<HotspotScript>().realWorldPosition;
    //     float localX = realHotspotPos.x - realOriginPos.x;
    //     float localY = realHotspotPos.y - realOriginPos.y;
    //     localX *= modelScale;
    //     localY *= modelScale;
    //     Vector3 offset = new Vector3(localX, 0f, localY);
    //     hotspot.transform.SetParent(breakwaterOrigin.transform);
    //     hotspot.transform.localPosition = -offset;
    // }

    [SerializeField] float rotationOffsetDegrees; // tweak in inspector

    void ChangeHotspotPosition(GameObject hotspot)
    {
        HotspotScript hs = hotspot.GetComponent<HotspotScript>();

        Vector3 localOffset = new Vector3(
            -(hs.realWorldPosition.x - realOriginPos.x) * modelScale,
            0f,
            -(hs.realWorldPosition.y - realOriginPos.y) * modelScale
        );

        Quaternion correction = Quaternion.Euler(0f, rotationOffsetDegrees, 0f);
        localOffset = correction * localOffset;

        hotspot.transform.SetParent(breakwaterOrigin.transform, false);
        hotspot.transform.localPosition = localOffset;
    }

    GameObject CreateHotspot(string ID, Vector2 realPos)
    {
        GameObject newHotspot = Instantiate(hotspotPrefab);

        newHotspot.name = "Hotspot_" + ID;

        HotspotScript hs = newHotspot.GetComponent<HotspotScript>();
        hs.hotspotID = int.Parse(ID);
        hs.realWorldPosition = new Vector2(realPos.x, realPos.y);
        hs.UI_Manager = view_UI_manager;

        hotspots.Add(hs);

        return newHotspot;
    }

    void AssignZone(GameObject hotspot)
    {
        HotspotScript hs = hotspot.GetComponent<HotspotScript>();

        float zPos = -(hotspot.transform.localPosition.z + hotspot.transform.parent.localPosition.z);

        if (zPos < 70f)
            hs.troco_ID = 'D';
        else if (zPos < 255f)
            hs.troco_ID = 'C';
        else if (zPos < 415f)
            hs.troco_ID = 'B';
        else
            hs.troco_ID = 'A';
    }

    public void SetUIManager(UI_Manager ui_Manager)
    {
        view_UI_manager = ui_Manager;
        Debug.Log("1");
    }

    public void ShowOnlyZoneHotspots(string zone)
    {
        switch (zone)
        {
            case "A":
                HideHotspotsNotFromZone('A');
                break;
            case "B":
                HideHotspotsNotFromZone('B');
                break;
            case "C":
                HideHotspotsNotFromZone('C');
                break;
            case "D":
                HideHotspotsNotFromZone('D');
                break;
            default:
                ShowAllHotspots();
                break;
        }
    }

    private void HideHotspotsNotFromZone(char zoneID)
    {
        //HotspotScript[] hotspots = breakwaterOrigin.GetComponentsInChildren<HotspotScript>();

        foreach (HotspotScript hotspot in hotspots)
        {
            if (hotspot.troco_ID != zoneID)
            {
                hotspot.gameObject.SetActive(false);
            }
            else
            {
                hotspot.gameObject.SetActive(true);
            }
        }
    }

    private void ShowAllHotspots()
    {
        //HotspotScript[] hotspots = breakwaterOrigin.GetComponentsInChildren<HotspotScript>();

        foreach (HotspotScript hotspot in hotspots)
        {
            hotspot.gameObject.SetActive(true);
        }
    }
}
