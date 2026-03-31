using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Zone
{
    public string Id;
    public int[] bounds;
    public List<HotspotScript> Hotspots;
    public List<Inspection> Inspections;
    public ZoneCharacteristics Caracteristics;
    public Inspection lastInspection;
    public Inspection referenceInspection;
    public int riskLevel;
    public int resistentArmorLayerLevel;
    public int interiorArmorLayerLevel;
    public int superStructureLayerLevel;

    //Based on the year of the model it calculates the risk level of the zone
    public void prepareRiskLevel(int modelYear)
    {
        int refInspectionYear = modelYear -5;
        foreach (Inspection inspection in Inspections)
        {
            if (inspection.Year == modelYear)
                lastInspection = inspection;
            if (inspection.Year == refInspectionYear)
                referenceInspection = inspection;
        }

        resistentArmorLayerLevel = CalculateResistantArmorLayerRiskLevel();
        interiorArmorLayerLevel = CalculateInteriorArmorLayerRiskLevel();
        superStructureLayerLevel = CalculateSuperstructureRiskLevel();
        riskLevel = resistentArmorLayerLevel;

        if (interiorArmorLayerLevel > riskLevel)
            riskLevel = interiorArmorLayerLevel;

        if (superStructureLayerLevel > riskLevel)
            riskLevel = superStructureLayerLevel;
    }

    private int CalculateResistantArmorLayerRiskLevel()
    {
        int currentLevel =
            lastInspection.ResistentArmorLayer.DamageLevel;

        int sumLevel =
            currentLevel +
            referenceInspection.ResistentArmorLayer.DamageLevel;

        if (currentLevel == 5)
            return 5;

        if (sumLevel == 2 * currentLevel && currentLevel < 5)
            return 0;

        if (sumLevel == 3 && currentLevel == 2)
            return 1;

        if (sumLevel == 1)
            return 1;

        if (sumLevel == 7)
            return 2;

        if (sumLevel == 5 && currentLevel == 3)
            return 2;

        if (sumLevel == 4 && currentLevel == 3)
            return 3;

        if (sumLevel == 2 && currentLevel == 2)
            return 3;

        if (sumLevel == 4 && currentLevel == 4)
            return 4;

        if (sumLevel == 5 && currentLevel == 4)
            return 4;

        if (sumLevel == 6 && currentLevel == 4)
            return 4;

        if (sumLevel == 3 && currentLevel == 3)
            return 4;

        // default fallback
        return 0;
    }

    private int CalculateInteriorArmorLayerRiskLevel()
    {
        int currentLevel =
            lastInspection.InteriorArmorLayer.DamageLevel;

        int referenceLevel =
            referenceInspection.InteriorArmorLayer.DamageLevel;

        int sumLevel = currentLevel + referenceLevel;

        if (currentLevel == 5)
            return 5;

        if (sumLevel == 2 * currentLevel)
            return 0;

        if (sumLevel == 3 && currentLevel == 2)
            return 1;

        if (sumLevel == 1)
            return 1;

        if (sumLevel == 7)
            return 2;

        if (sumLevel == 5 && currentLevel == 3)
            return 2;

        if ((sumLevel == 4 && currentLevel == 3) ||
            (sumLevel == 2 && currentLevel == 2))
            return 3;

        if ((sumLevel == 4 && currentLevel == 4) ||
            (sumLevel == 5 && currentLevel == 4) ||
            (sumLevel == 6 && currentLevel == 4) ||
            (sumLevel == 3 && currentLevel == 3))
            return 4;

        return 0;
    }

    private int CalculateSuperstructureRiskLevel()
    {
        int currentLevel =
            lastInspection.Superstructure.DamageLevel;

        //confirmar
        int sumLevel =
            referenceInspection.Superstructure.DamageLevel;

        // risk level 0
        if (
            (currentLevel == 0 && sumLevel == 0) ||
            (currentLevel == 1 && sumLevel == 0) ||
            (currentLevel == 1 && sumLevel == 1) ||
            (currentLevel == 2 && sumLevel == 0)
        )
            return 0;

        // risk level 1
        if (
            (currentLevel == 2 && sumLevel == 1) ||
            (currentLevel == 3 && sumLevel == 0) ||
            (currentLevel == 2 && sumLevel == 2)
        )
            return 1;

        // risk level 2
        if (
            (currentLevel == 2 && sumLevel == 3) ||
            (currentLevel == 3 && sumLevel == 1) ||
            (currentLevel == 3 && sumLevel == 2) ||
            (currentLevel == 4 && sumLevel == 0) ||
            (currentLevel == 4 && sumLevel == 1)
        )
            return 2;

        return 0;
    }
}
