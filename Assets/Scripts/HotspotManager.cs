using UnityEngine;
using System.IO;
using System.Globalization;
public class HotspotManager : MonoBehaviour
{
    [SerializeField] GameObject breakwaterOrigin;
    [SerializeField] float modelScale;
    Vector3 realOriginPos;
    [SerializeField] GameObject hotspotPrefab;

    [SerializeField] GameObject imageUI;

    void Start()
    {
        realOriginPos = breakwaterOrigin.GetComponent<originPointScript>().realWorldPosition;
        ReadCSV();
    }

    void ReadCSV()
    {
        string path = Path.Combine(Application.dataPath, "HotspotData/Hotspot_Data.csv");

        if (!File.Exists(path))
        {
            Debug.LogError("CSV file not found: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        // Start from line index 2
        for (int i = 2; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] columns = lines[i].Split(',');

            string id = columns[0];

            float x = float.Parse(columns[1], CultureInfo.InvariantCulture);
            float y = float.Parse(columns[2], CultureInfo.InvariantCulture);

            //Debug.Log($"ID: {id} | X: {x} | Y: {y}");

            GameObject hotspot = CreateHotspot(id, new Vector2(x, y));
            ChangeHotspotPosition(hotspot);
        }
    }

    void ChangeHotspotPosition(GameObject hotspot)
    {
        Vector3 realHotspotPos = hotspot.GetComponent<HotspotScript>().realWorldPosition;
        float localX = realHotspotPos.x - realOriginPos.x;
        float localY = realHotspotPos.y - realOriginPos.y;
        localX *= modelScale;
        localY *= modelScale;
        Vector3 offset = new Vector3(localX, 0f, localY);
        hotspot.transform.SetParent(breakwaterOrigin.transform);
        hotspot.transform.localPosition = -offset;
    }

    GameObject CreateHotspot(string ID, Vector2 realPos)
    {
        GameObject newHotspot = Instantiate(hotspotPrefab);

        newHotspot.name = "Hotspot_" + ID;

        HotspotScript hs = newHotspot.GetComponent<HotspotScript>();
        hs.hotspotID = int.Parse(ID);
        hs.realWorldPosition = new Vector2(realPos.x, realPos.y);
        hs.hotspotImageObj = imageUI;

        //mais tarde meter tb o ID do troço a que pertence

        return newHotspot;
    }
}
