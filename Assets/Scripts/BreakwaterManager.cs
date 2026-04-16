using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;


public class BreakwaterManager : MonoBehaviour
{
    private Vector3 OriginWalkingPoint;
    private Vector3 RefHotspotPoint;
    [SerializeField] GameObject modelPrefab;

    [SerializeField] GameObject HighlightFolder;
    public List<Zone> Zones;
    public int modelInspectionYear;
    [SerializeField] bool showHighlight;

    [SerializeField] Renderer overlayRenderer;
    private string selectedZoneId = null;
    Dictionary<string, Renderer> highlightRenderers;

    //[SerializeField] string jsonFile;

    string path;
    string json;
    string json2;


    void Awake()
    {
        path = Path.Combine(Application.streamingAssetsPath, "json.json");
        json = File.ReadAllText(path);
        path = Path.Combine(Application.streamingAssetsPath, "json2.json");
        json2 = File.ReadAllText(path);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // com versao de ler json inicial
        Zones = BuildAll(json);
        List<Inspection> inspections = BuildInspections(json2);
        AssignInspectionsToZones(Zones,inspections);
        PrepareRiskLevel();

        highlightRenderers = new Dictionary<string, Renderer>();

        foreach (Zone zone in Zones)
        {
            GameObject obj = GameObject.Find("Highlight_" + zone.Id);
            if (obj != null)
            {
                highlightRenderers[zone.Id] = obj.GetComponent<Renderer>();
            }
        }

        foreach (Zone zone in Zones)
        {
            if (!highlightRenderers.ContainsKey(zone.Id))
                continue;

            Renderer r = highlightRenderers[zone.Id];

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);

            mpb.SetColor("_Color", RiskToColor(zone.riskLevel));

            r.SetPropertyBlock(mpb);
        }
        // if (showHighlight)
        // {
        //     HighlightFolder.SetActive(true);
        // }
        // else
        // {
        //     HighlightFolder.SetActive(false);
        // }
        ApplyHighlights();
    }

    void ApplyHighlights()
    {
        foreach (Zone zone in Zones)
        {
            if (!highlightRenderers.ContainsKey(zone.Id))
                continue;

            GameObject obj = highlightRenderers[zone.Id].gameObject;

            bool isSelected = selectedZoneId == zone.Id;

            if (selectedZoneId != null)
            {
                obj.SetActive(isSelected);
            }
            else
            {
                obj.SetActive(showHighlight);
            }
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

    // Update is called once per frame
    void Update()
    {
        
    }

    void PrepareRiskLevel()
    {
        foreach (Zone zone in Zones)
        {
            zone.prepareRiskLevel(modelInspectionYear);
            Debug.Log("Risk Level " + zone.riskLevel.ToString());
        }
    }

    public static List<Zone> BuildAll(string json)
    {
        var jArray = JArray.Parse(json);

        var zones = new List<Zone>();

        foreach (JObject jObject in jArray)
        {
            zones.Add(new Zone
            {
                Id = jObject["Nome do Troço"]?.ToString(),
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

        // YEAR
        inspection.Year = ParseYear(jObject["Data"]);
        // var date = jObject["Data"]?.ToString();
        // if (!string.IsNullOrEmpty(date))
        // {
        //     var parts = date.Split('-');
        //     if (parts.Length == 3)
        //         inspection.Year = int.Parse(parts[2]);
        // }
        // Debug.Log("Year: " + inspection.Year.ToString());

        // var obsArray = jObject["observacoes"] as JArray;

        // string observation = null;

        // if (obsArray != null)
        // {
        //     foreach (var item in obsArray)
        //     {
        //         var value = item.ToString();

        //         if (!string.IsNullOrWhiteSpace(value))
        //         {
        //             observation = value;
        //             break;
        //         }
        //     }
        // }

        // inspection.ResistentArmorLayer.Observation = observation;


        inspection.ZoneId = jObject["Nome do Troço"]?.ToString();
        //Debug.Log("inspection zone " + inspection.ZoneId.ToString());

        inspection.General = Map<GeneralInspection>(jObject, GeneralInspectionMap.Map);

        inspection.ResistentArmorLayer.DamageLevel =
            ParseDamageLevel(jObject["GrauMan"]);

        inspection.Superstructure.DamageLevel =
            ParseDamageLevel(jObject["GrauSup"]);

        inspection.InteriorArmorLayer.DamageLevel =
            ParseDamageLevel(jObject["GrauTar"]);

        inspection.ResistentArmorLayer =
            Map<ResistentArmorLayerInspection>(jObject, ResistentArmorLayerInspectionMap.Map);

        inspection.Superstructure =
            Map<SuperstructureInspection>(jObject, SuperstructureInspectionMap.Map);

        inspection.InteriorArmorLayer =
            Map<InteriorArmorLayerInspection>(jObject, InteriorArmorLayerInspectionMap.Map);
        
        return inspection;
    }

    public static int ParseYear(JToken token)
    {
        if (token == null) return 0;

        var str = token.ToString();

        if (string.IsNullOrWhiteSpace(str))
            return 0;

        // "01-01-2018"
        var parts = str.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out int year))
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

    public static List<Inspection> BuildInspections(string json)
    {
        var jArray = JArray.Parse(json);
        var inspections = new List<Inspection>();

        foreach (JObject jObject in jArray)
        {
            inspections.Add(BuildInspection(jObject));
        }

        return inspections;
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

            // EVERYTHING becomes string
            var value = token.ToString();

            field.SetValue(obj, value);
            Debug.Log($"[MAP OK] {jsonKey} → {fieldName} = {value}");
        }
        Debug.Log($"[MAP END] Finished mapping {typeof(T).Name}");
        return obj;
    }
}
