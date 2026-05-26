using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;
using System.Collections;

public class BreakwaterManager : MonoBehaviour
{
    private Vector3 OriginWalkingPoint;
    private bool isInitialized = false;
    private Vector3 RefHotspotPoint;
    [SerializeField] GameObject modelPrefab;

    [SerializeField] GameObject HighlightFolder;
    public List<Zone> Zones;
    public int modelInspectionYear;
    [SerializeField] bool showHighlight;

    [SerializeField] Renderer overlayRenderer;
    private string selectedZoneId = null;
    Dictionary<string, Renderer> highlightRenderers;

    [SerializeField] int codEstrutura;

    int heightHighlightChange;
    [SerializeField] ControllerManager controller;
    bool useCubesRuntime;

    string path;
    string json;

    void Awake()
    {
        path = Path.Combine(Application.streamingAssetsPath, "osom_dados.json");
        StartCoroutine(LoadJson());
    }

    void Initialize()
    {
        heightHighlightChange = 25;
        Zones = BuildAll(json);
        List<Inspection> inspections = BuildInspections(json);
        AssignInspectionsToZones(Zones, inspections);

        PrepareRiskLevel();

        highlightRenderers = new Dictionary<string, Renderer>();

        foreach (Zone zone in Zones)
        {
            GameObject obj = GameObject.Find("Highlight_" + zone.name);
            if (obj != null)
            {
                highlightRenderers[zone.name] = obj.GetComponent<Renderer>();
            }
        }

        foreach (Zone zone in Zones)
        {
            if (!highlightRenderers.ContainsKey(zone.name))
                continue;

            Renderer r = highlightRenderers[zone.name];

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);

            mpb.SetColor("_Color", RiskToColor(zone.riskLevel));

            r.SetPropertyBlock(mpb);
        }
        SetupShaderColors();
        ApplyHighlights();
        UpdateHighlightMode();
        isInitialized = true;
    }

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     heightHighlightChange = 25;
    //     Zones = BuildAll(json);
    //     List<Inspection> inspections = BuildInspections(json);
    //     AssignInspectionsToZones(Zones,inspections);
        
    //     PrepareRiskLevel();

    //     highlightRenderers = new Dictionary<string, Renderer>();

    //     foreach (Zone zone in Zones)
    //     {
    //         GameObject obj = GameObject.Find("Highlight_" + zone.name);
    //         if (obj != null)
    //         {
    //             highlightRenderers[zone.name] = obj.GetComponent<Renderer>();
    //         }
    //     }

    //     foreach (Zone zone in Zones)
    //     {
    //         if (!highlightRenderers.ContainsKey(zone.name))
    //             continue;

    //         Renderer r = highlightRenderers[zone.name];

    //         MaterialPropertyBlock mpb = new MaterialPropertyBlock();
    //         r.GetPropertyBlock(mpb);

    //         mpb.SetColor("_Color", RiskToColor(zone.riskLevel));

    //         r.SetPropertyBlock(mpb);
    //     }
    //     SetupShaderColors();
    //     ApplyHighlights();
    //     UpdateHighlightMode();
    // }

    void SetupShaderColors()
    {
        if (overlayRenderer == null) return;

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        overlayRenderer.GetPropertyBlock(mpb);

        foreach (Zone zone in Zones)
        {
            Color c = RiskToColor(zone.riskLevel);

            switch (zone.name)
            {
                case "D": mpb.SetColor("_colorA", c); break;
                case "C": mpb.SetColor("_colorB", c); break;
                case "B": mpb.SetColor("_colorC", c); break;
                case "A": mpb.SetColor("_colorD", c); break;
            }
        }

        overlayRenderer.SetPropertyBlock(mpb);
    }

    bool lastCubesState = false;

    void Update()
    {
        if (!isInitialized) return;

        float height = controller.GetHeight();
        bool newState = height >= heightHighlightChange;

        if (newState != lastCubesState)
        {
            useCubesRuntime = newState;
            UpdateHighlightMode();
            lastCubesState = newState;
        }
    }

    public void SetHeightThreshold(float value)
    {
        heightHighlightChange = Mathf.RoundToInt(value);
        Debug.Log(heightHighlightChange);
    }

    void UpdateHighlightMode()
    {
        ShowHighlightCubes(useCubesRuntime);
        ShowHighlightShader(!useCubesRuntime);
    }
    void ShowHighlightCubes(bool state)
    {
        foreach (var kvp in highlightRenderers)
        {
            if (kvp.Value != null)
            {
                kvp.Value.gameObject.SetActive(state && showHighlight);
            }
        }

        if (useCubesRuntime)
        {
            foreach (Zone zone in Zones)
            {
                if (!highlightRenderers.ContainsKey(zone.name))
                    continue;

                GameObject obj = highlightRenderers[zone.name].gameObject;

                bool isSelected = selectedZoneId == zone.name;

                // ONLY handle selection logic here
                obj.SetActive(isSelected || selectedZoneId == null);
            }
        }
    }

    void ApplyHighlights()
    {
        UpdateShaderSelection();
        if (useCubesRuntime)
        {
            foreach (Zone zone in Zones)
            {
                if (!highlightRenderers.ContainsKey(zone.name))
                    continue;

                GameObject obj = highlightRenderers[zone.name].gameObject;

                bool isSelected = selectedZoneId == zone.name;

                obj.SetActive(isSelected || selectedZoneId == null);
            }
        }
    }

    void ShowHighlightShader(bool state)
    {
        if (overlayRenderer == null)
            return;

        overlayRenderer.material.SetFloat("_strenght", state ? 0.3f : 0f);
    }

    void UpdateShaderSelection()
    {
        if (overlayRenderer == null)
            return;

        int index = GetZoneIndex(selectedZoneId);

        Debug.Log("hello " + index);
        overlayRenderer.material.SetInt("_selectedZone", index);
    }

    private int GetZoneIndex(string zoneName)
    {
        switch (zoneName)
        {
            case "A": return 0;
            case "B": return 1;
            case "C": return 2;
            case "D": return 3;
            default: return -1;
        }
    }

    public void HideZone(string zoneID)
    {
        // hide hotspots que pertencem a zona quando a adicionar
        bool exists = highlightRenderers.ContainsKey(zoneID);

        if (!exists)
        {
            Debug.LogWarning($"Zone {zoneID} not found → reverting to default view");

            selectedZoneId = null; // fallback to default
        }
        else
        {
            selectedZoneId = zoneID;
        }

        ApplyHighlights();
    }

    public void ClearSelection()
    {
        selectedZoneId = null;
        ApplyHighlights();
    }

    Color RiskToColor(int riskLevel)
    {
        switch (riskLevel)
        {
            case 0: return Color.blue;
            case 1: return Color.cyan;
            case 2: return Color.orange;
            case 3: return Color.red;
            case 4: return Color.black;
            default: return Color.white;
        }
    }

    void PrepareRiskLevel()
    {
        foreach (Zone zone in Zones)
        {
            zone.PrepareRiskLevel(modelInspectionYear);
            Debug.Log("Risk Level " + zone.riskLevel.ToString());
        }
    }

    IEnumerator LoadJson()
    {
        string uri = Path.Combine(Application.streamingAssetsPath, "osom_dados.json");

        if (!uri.Contains("://"))
            uri = "file://" + uri;

        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("JSON load failed: " + request.error);
                yield break;
            }

            json = request.downloadHandler.text;

            Debug.Log("JSON loaded, size: " + json.Length);

            Initialize(); // your setup
        }
    }

    public List<Zone> BuildAll(string json)
    {
        var root = JObject.Parse(json);
        var trocos = root["trocos"] as JObject;

        var zones = new List<Zone>();

        foreach (var prop in trocos.Properties())
        {
            JObject jObject = (JObject)prop.Value;

            // filter for codEstrutura
            int cod = int.Parse(jObject["codEstrutura"]?.ToString() ?? "0");

            if (cod != codEstrutura)
                continue;

            zones.Add(new Zone
            {
                Id = int.Parse(jObject["codTroco"]?.ToString() ?? "0"),
                name = jObject["nomeTroco"]?.ToString(),
                Caracteristics = new ZoneCharacteristics
                {
                    General = Map<GeneralZoneData>(jObject, GeneralZoneDataMap.Map),
                    Superstructure = Map<SuperstructureData>(jObject, SuperstructureDataMap.Map),
                    InnerCrestBerm = Map<InnerCrestBermData>(jObject, InnerCrestBermDataMap.Map),
                    InteriorArmorLayer = Map<InteriorArmorLayerData>(jObject, InteriorArmorLayerDataMap.Map),
                    OuterCrestBerm = Map<OuterCrestBermData>(jObject, OuterCrestBermDataMap.Map),
                    ResistentArmorLayer = Map<ResistentArmorLayerData>(jObject, ResistentArmorLayerDataMap.Map),
                    ToeBerm = Map<ToeBermData>(jObject, ToeBermDataMap.Map),
                    Foundation = Map<FoundationData>(jObject, FoundationDataMap.Map)
                }
            });
        }

        return zones;
    }

    public List<Inspection> BuildInspections(string json)
    {
        var root = JObject.Parse(json);
        var trocos = root["observacoes"] as JObject;
        var inspections = new List<Inspection>();

        foreach (var prop in trocos.Properties())
        {
            JObject jObject = (JObject)prop.Value;
            
            // FILTER by codEstrutura
            int cod = int.Parse(jObject["codEstrutura"]?.ToString() ?? "0");

            if (cod != codEstrutura)
                continue;

            var inspection = BuildInspection(jObject);
            inspections.Add(inspection);
        }

        return inspections;
    }

    public static Inspection BuildInspection(JObject jObject)
    {
        var inspection = new Inspection
        {
            ResistentArmorLayer = new ResistentArmorLayerInspection(),
            Superstructure = new SuperstructureInspection(),
            InteriorArmorLayer = new InteriorArmorLayerInspection(),
            General = new GeneralInspection(),
            Underwater = new UnderwaterInspection(),
            Pier = new PierInspection()
        };

        inspection.Year = ParseYear(jObject["data"]);

        Debug.Log("Year: " + inspection.Year);
        inspection.ZoneId = int.Parse(jObject["codTroco"]?.ToString() ?? "0");
        
        inspection.General = Map<GeneralInspection>(jObject, GeneralInspectionMap.Map);

        inspection.ResistentArmorLayer =
            Map<ResistentArmorLayerInspection>(jObject, ResistentArmorLayerInspectionMap.Map);

        inspection.Superstructure =
            Map<SuperstructureInspection>(jObject, SuperstructureInspectionMap.Map);

        inspection.InteriorArmorLayer =
            Map<InteriorArmorLayerInspection>(jObject, InteriorArmorLayerInspectionMap.Map);
        
        inspection.Underwater = 
            Map<UnderwaterInspection>(jObject, UnderwaterInspectionMap.Map);

        inspection.Pier =
            Map<PierInspection>(jObject, PierInspectionMap.Map);

        return inspection;
    }

    public static int ParseYear(JToken token)
    {
        if (token == null) return 0;

        var str = token.ToString();

        if (string.IsNullOrWhiteSpace(str))
            return 0;

        // "2018-01-01"
        var parts = str.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[0], out int year))
            return year;

        return 0;
    }

    public static int ParseDamageLevel(JToken token)
    {
        if (token == null)
            return 0;

        var str = token.ToString();

        if (string.IsNullOrWhiteSpace(str))
            return 0;

        if (int.TryParse(str, out int value))
            return value;

        return 0;
    }

    public static void AssignInspectionsToZones(List<Zone> zones, List<Inspection> inspections)
    {
        foreach (var zone in zones)
        {
            zone.Inspections = new List<Inspection>();

            foreach (var inspection in inspections)
            {
                if (inspection.ZoneId == zone.Id)
                {
                    zone.Inspections.Add(inspection);
                }
            }
        }
    }

    public static T Map<T>(JObject json, Dictionary<string, string> map) where T : new()
    {
        T obj = new T();

        Debug.Log($"[MAP START] Mapping type: {typeof(T).Name}");
        foreach (var entry in map)
        {
            var jsonKey = entry.Key;
            var fieldName = entry.Value;

            var token = json[jsonKey];
            if (token == null){
                Debug.LogWarning($"[MAP MISS] JSON key not found: {jsonKey}");
                continue;
            }
            var field = typeof(T).GetField(fieldName);
            if (field == null)
            {
                Debug.LogWarning($"[MAP MISS] Field not found in {typeof(T).Name}: {fieldName}");
                continue;
            }

            // everything becomes string
            var value = token.ToString();

            field.SetValue(obj, value);
            Debug.Log($"[MAP OK] {jsonKey} → {fieldName} = {value}");
        }
        Debug.Log($"[MAP END] Finished mapping {typeof(T).Name}");
        return obj;
    }

    public Zone GetZone(string zoneName)
    {
        foreach (var zone in Zones)
        {
            if (zone.name == zoneName)
                return zone;
        }
        return null;
    }

}
