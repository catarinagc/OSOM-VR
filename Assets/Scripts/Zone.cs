using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Zone
{
    public int Id;
    public string name;
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

    private int FormulaEvolucao(int estadoAtual, int estadoAnterior) {
        if ((estadoAtual == null) || (estadoAnterior == null)) return -1;
        var soma = estadoAtual + estadoAnterior;
        if (estadoAtual == 5)				return 5; // x -> 5
        if (soma == 2 * estadoAtual)		return 0; // x -> x
        if (soma == 3 && estadoAtual == 2)	return 1; // 1 -> 2
        if (soma == 1)						return 1; // 0 -> 1 && 1 -> 0 !
        if (soma == 7)						return 2; // 3 -> 4 && 4 -> 3 ! && 5 -> 2 !
        if (soma == 5 && estadoAtual == 3)	return 2; // 2 -> 3
        if (soma == 4 && estadoAtual == 3)	return 3; // 1 -> 3
        if (soma == 2 && estadoAtual == 2)	return 3; // 0 -> 2
        if (soma == 4 && estadoAtual == 4)	return 4; // 0 -> 4
        if (soma == 5 && estadoAtual == 4)	return 4; // 1 -> 4
        if (soma == 6 && estadoAtual == 4)	return 4; // 2 -> 4
        if (soma == 3 && estadoAtual == 3)	return 4; // 0 -> 3
        return 0;
    }

    private int FormulaRisco(int estado, int evolucao) {
        if ((estado == null) || (evolucao == null)) return -1;
        if (estado == 0 && evolucao == 0) return 0;
        if (estado == 1 && evolucao == 0) return 0;
        if (estado == 1 && evolucao == 1) return 0;
        if (estado == 2 && evolucao == 0) return 0;
        if (estado == 2 && evolucao == 1) return 1;
        if (estado == 3 && evolucao == 0) return 1;
        if (estado == 2 && evolucao == 2) return 1;
        if (estado == 2 && evolucao == 3) return 2;
        if (estado == 3 && evolucao == 1) return 2;
        if (estado == 3 && evolucao == 2) return 2;
        if (estado == 4 && evolucao == 0) return 2;
        if (estado == 4 && evolucao == 1) return 2;
        if (estado == 3 && evolucao == 3) return 3;
        if (estado == 3 && evolucao == 4) return 3;
        if (estado == 4 && evolucao == 2) return 3;
        if (estado == 4 && evolucao == 3) return 3;
        if (estado == 4 && evolucao == 4) return 3;
        if (estado == 5 && evolucao == 5) return 4;
        return 0;
    }

    //Based on OSOM the year of the model it calculates the risk level of the zone
    public void PrepareRiskLevel(int modelYear)
    {
        int refInspectionYear = modelYear -5;
        foreach (Inspection inspection in Inspections)
        {
            if (inspection.Year == modelYear)
                lastInspection = inspection;
            if (inspection.Year == refInspectionYear)
                referenceInspection = inspection;
        }

        resistentArmorLayerLevel = FormulaRisco(lastInspection.ResistentArmorLayer.CalculateDamageLevel(), 
            FormulaEvolucao(lastInspection.ResistentArmorLayer.CalculateDamageLevel(), referenceInspection.ResistentArmorLayer.CalculateDamageLevel()));
        
        interiorArmorLayerLevel = FormulaRisco(lastInspection.InteriorArmorLayer.CalculateDamageLevel(), 
            FormulaEvolucao(lastInspection.InteriorArmorLayer.CalculateDamageLevel(), referenceInspection.InteriorArmorLayer.CalculateDamageLevel()));

        superStructureLayerLevel = FormulaRisco(lastInspection.Superstructure.CalculateDamageLevel(), 
            FormulaEvolucao(lastInspection.Superstructure.CalculateDamageLevel(), referenceInspection.Superstructure.CalculateDamageLevel()));
        
        riskLevel = resistentArmorLayerLevel;

        if (interiorArmorLayerLevel > riskLevel)
            riskLevel = interiorArmorLayerLevel;

        if (superStructureLayerLevel > riskLevel)
            riskLevel = superStructureLayerLevel;
    }
}
