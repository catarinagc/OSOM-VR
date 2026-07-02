using UnityEngine;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class HotspotManager : MonoBehaviour
{
    // Singleton-style access so HotspotScript / Image_UI_Manager / ImageDisplayController
    // can request and release images without needing a serialized reference.
    public static HotspotManager Instance { get; private set; }

    [SerializeField] GameObject breakwaterOrigin;
    [SerializeField] float modelScale;
    Vector3 realOriginPos;
    [SerializeField] GameObject hotspotPrefab;
    private List<HotspotScript> hotspots = new List<HotspotScript>();
    //[SerializeField] UI_Manager UI_Manager;
    private UI_Manager view_UI_manager;
    private string spritesRoot;

    public static bool IsReady { get; private set; } = false;
    private bool csvDone = false;

    // ---------------------------------------------------------------
    // Per-hotspot lazy loading + reference counting.
    //
    // Images are no longer preloaded at game start. Instead, each
    // hotspot's images are loaded the first time it's requested
    // (via OnInteract), tagged with an Addressables Label of the
    // form "HS_<hotspotID>" (see the AssignHotspotLabels editor tool).
    //
    // Because VR lets a player drag an image out into the world and
    // leave it open independently of the main gallery panel, we can't
    // release on a simple "panel closed" event. Instead every consumer
    // (main panel open, each dragged-out copy) increments a reference
    // count, and decrements it when done. Only when a hotspot's count
    // reaches zero do we release its Addressables handles.
    // ---------------------------------------------------------------
    private Dictionary<int, List<AsyncOperationHandle<Sprite>>> loadedSpriteHandles = new();
    private Dictionary<int, List<InspectionImage>> cachedImages = new();
    private Dictionary<int, int> refCounts = new();
    private Dictionary<int, List<Action<List<InspectionImage>>>> pendingCallbacks = new();

    void Awake()
    {
        Instance = this;
        IsReady = false;
        csvDone = false;
    }

    void OnEnable()
    {
        XRModeSwitcher.OnModeSelected += OnModeChosen;
    }

    void OnDisable()
    {
        XRModeSwitcher.OnModeSelected -= OnModeChosen;
    }

    void OnDestroy()
    {
        // Scene/app is going away - release everything still held.
        foreach (var handles in loadedSpriteHandles.Values)
        {
            foreach (var h in handles)
            {
                if (h.IsValid())
                    Addressables.Release(h);
            }
        }
        loadedSpriteHandles.Clear();
        cachedImages.Clear();
        refCounts.Clear();

        if (Instance == this)
            Instance = null;
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

    // -----------------------------------------------------------
    // Public API used by HotspotScript / Image_UI_Manager
    // -----------------------------------------------------------

    /// <summary>
    /// Requests the images for a hotspot. Every call to this MUST be paired
    /// with exactly one later call to ReleaseHotspotReference(hotspotID) -
    /// once for the main panel open, and once more for every dragged-out
    /// VR copy created from that hotspot's images.
    /// </summary>
    public void RequestHotspotImages(int hotspotID, Action<List<InspectionImage>> onReady)
    {
        AddHotspotReference(hotspotID);

        if (cachedImages.TryGetValue(hotspotID, out var cached))
        {
            onReady?.Invoke(cached);
            return;
        }

        if (pendingCallbacks.TryGetValue(hotspotID, out var queued))
        {
            queued.Add(onReady);
            return;
        }

        pendingCallbacks[hotspotID] = new List<Action<List<InspectionImage>>> { onReady };
        StartCoroutine(LoadHotspotImagesRoutine(hotspotID));
    }

    public void AddHotspotReference(int hotspotID)
    {
        refCounts.TryGetValue(hotspotID, out int current);
        refCounts[hotspotID] = current + 1;
    }

    public void ReleaseHotspotReference(int hotspotID)
    {
        if (!refCounts.ContainsKey(hotspotID))
            return;

        refCounts[hotspotID]--;

        if (refCounts[hotspotID] <= 0)
        {
            refCounts.Remove(hotspotID);

            if (loadedSpriteHandles.TryGetValue(hotspotID, out var handles))
            {
                foreach (var h in handles)
                {
                    if (h.IsValid())
                        Addressables.Release(h);
                }
                loadedSpriteHandles.Remove(hotspotID);
            }

            cachedImages.Remove(hotspotID);
            Debug.Log($"[HM] Released images for hotspot {hotspotID}");
        }
    }

    private IEnumerator LoadHotspotImagesRoutine(int hotspotID)
    {
        string label = $"HS_{hotspotID}";

        AsyncOperationHandle<IList<IResourceLocation>> locHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Sprite));
        yield return locHandle;

        if (locHandle.Status != AsyncOperationStatus.Succeeded || locHandle.Result.Count == 0)
        {
            Debug.LogError($"[HM] No Addressables content found for label '{label}'");
            if (locHandle.IsValid()) Addressables.Release(locHandle);
            FlushPending(hotspotID, new List<InspectionImage>());
            yield break;
        }

        Debug.Log($"[HM] Label '{label}' resolved {locHandle.Result.Count} locations:");
        foreach (var loc in locHandle.Result)
            Debug.Log($"[HM]     -> {loc.PrimaryKey} (ResourceType: {loc.ResourceType})");

        List<InspectionImage> images = new List<InspectionImage>();
        List<AsyncOperationHandle<Sprite>> spriteHandles = new List<AsyncOperationHandle<Sprite>>();

        foreach (var loc in locHandle.Result)
        {
            AsyncOperationHandle<Sprite> spriteHandle = Addressables.LoadAssetAsync<Sprite>(loc);
            yield return spriteHandle;

            if (spriteHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[HM] Failed to load sprite at '{loc.PrimaryKey}'");
                continue;
            }

            spriteHandles.Add(spriteHandle);

            if (spriteHandle.Result.texture != null)
            {
                spriteHandle.Result.texture.mipMapBias = -1f;
                Debug.Log($"Bias right after set: {spriteHandle.Result.texture.mipMapBias}");
                StartCoroutine(LogBiasNextFrame(spriteHandle.Result.texture));
                Debug.Log($"Mip count: {spriteHandle.Result.texture.mipmapCount}");
            }

            // The year-folder is still part of the asset's path even though
            // we no longer load by a year Label, so we recover it from there.
            string folderName = Path.GetFileName(Path.GetDirectoryName(loc.PrimaryKey));
            string year = folderName.Split('-')[0];

            string fileName = spriteHandle.Result.name;
            string imageDir = string.Empty;

            for (int i = 0; i < fileName.Length; i++)
            {
                if (!char.IsDigit(fileName[i]))
                {
                    imageDir = fileName[i..];
                    break;
                }
            }

            images.Add(new InspectionImage
            {
                sprite = spriteHandle.Result,
                hotspotID = hotspotID,
                dir = imageDir,
                year = year
            });
        }

        Addressables.Release(locHandle);

        // Newest year first.
        images.Sort((a, b) => int.Parse(b.year) - int.Parse(a.year));

        loadedSpriteHandles[hotspotID] = spriteHandles;
        cachedImages[hotspotID] = images;

        Debug.Log($"[HM] Loaded {images.Count} images for hotspot {hotspotID}");
        FlushPending(hotspotID, images);
    }

    IEnumerator LogBiasNextFrame(Texture tex)
    {
        yield return null;
        yield return new WaitForSeconds(1f);
        Debug.Log($"Bias after a second: {tex.mipMapBias}");
    }

    private void FlushPending(int hotspotID, List<InspectionImage> images)
    {
        if (pendingCallbacks.TryGetValue(hotspotID, out var callbacks))
        {
            pendingCallbacks.Remove(hotspotID);
            foreach (var cb in callbacks)
                cb?.Invoke(images);
        }
    }

    // -----------------------------------------------------------
    // CSV / hotspot creation (unchanged in behavior)
    // -----------------------------------------------------------

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
        Debug.Log($"[HM] ReadCSVDesktop started at {Time.time}");
        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found: " + path);
            return;
        }

        ProcessCSV(File.ReadAllLines(path));
        csvDone = true;
        IsReady = true;
        Debug.Log($"[HM] csvDone, IsReady=true at {Time.time}");
    }

    IEnumerator ReadCSVAndroid(string path)
    {
        UnityWebRequest req = UnityWebRequest.Get(path);
        yield return req.SendWebRequest();

        string[] lines = req.downloadHandler.text.Split('\n');
        ProcessCSV(lines);
        csvDone = true;
        IsReady = true;
        Debug.Log($"[HM] csvDone, IsReady=true at {Time.time}");
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
        //Debug.Log("1");
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
        foreach (HotspotScript hotspot in hotspots)
        {
            if (hotspot.troco_ID != zoneID)
            {
                hotspot.setTransparency(false);
            }
            else
            {
                hotspot.setTransparency(true);
            }
        }
    }

    private void ShowAllHotspots()
    {
        foreach (HotspotScript hotspot in hotspots)
        {
            hotspot.setTransparency(true);
        }
    }

    public List<HotspotScript> GetHotspotList()
    {
        return hotspots;
    }
}