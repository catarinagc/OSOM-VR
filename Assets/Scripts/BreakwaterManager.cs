using UnityEngine;
using System.Collections.Generic;

public class BreakwaterManager : MonoBehaviour
{
    private Vector3 OriginWalkingPoint;
    private Vector3 RefHotspotPoint;
    [SerializeField] GameObject modelPrefab;
    public List<Zone> Zones;
    public int modelInspectionYear;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
