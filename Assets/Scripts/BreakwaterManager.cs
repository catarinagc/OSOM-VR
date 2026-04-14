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

    Dictionary<string, Renderer> highlightRenderers;

    //[SerializeField] string jsonFile;

    string path;
    string json;


    void Awake()
    {
        path = Path.Combine(Application.streamingAssetsPath, "json.json");
        json = File.ReadAllText(path);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Zones = new List<Zone>
        // {
        //     new Zone { Id = "A", bounds = new int[] { -700, -417 }, riskLevel = 0 },
        //     new Zone { Id = "B", bounds = new int[] { -417, -257 }, riskLevel = 1 },
        //     new Zone { Id = "C", bounds = new int[] { -257, -70 }, riskLevel = 2 },
        //     new Zone { Id = "D", bounds = new int[] { -70, 20 }, riskLevel = 3 }
        // };

        // highlightRenderers = new Dictionary<string, Renderer>();

        // foreach (Zone zone in Zones)
        // {
        //     GameObject obj = GameObject.Find("Highlight_" + zone.Id);
        //     if (obj != null)
        //     {
        //         highlightRenderers[zone.Id] = obj.GetComponent<Renderer>();
        //     }
        // }

        // if (showHighlight)
        // {
        //     HighlightFolder.SetActive(true);
        //     foreach (Zone zone in Zones)
        //     {
        //         if (!highlightRenderers.ContainsKey(zone.Id))
        //             continue;

        //         Renderer r = highlightRenderers[zone.Id];

        //         MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        //         r.GetPropertyBlock(mpb);

        //         mpb.SetColor("_Color", RiskToColor(zone.riskLevel));

        //         r.SetPropertyBlock(mpb);
        //     }
        // }
        // else
        // {
        //     HighlightFolder.SetActive(false);
        // }


        // com versao de ler json inicial
        //Zones.Add(Build(json));
        //Zones = new List<Zone>();
        Zones = BuildAll(json);
    }

    Color RiskToColor(int riskLevel)
    {
        switch (riskLevel)
        {
            case 0: return Color.blue;
            case 1: return Color.red;
            case 2: return Color.magenta;
            case 3: return Color.cyan;
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
                Caracteristics = new ZoneCharacteristics
                {
                    General = Map<GeneralZoneData>(jObject, GeneralZoneDataMap.Map)
                }
            });
        }

        return zones;
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
