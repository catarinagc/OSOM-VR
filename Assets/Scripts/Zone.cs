using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Zone
{
    public class LayerUIData
    {
        public string Level;
        public string Evol;
        public string RefState;
        public string LastState;
    }
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
    public string resistentArmorLayerLevelText;
    public string resistentArmorLayerLevelTextSimple;
    public int interiorArmorLayerLevel;
    public string interiorArmorLayerLevelText;
    public string interiorArmorLayerLevelTextSimple;
    public int superStructureLayerLevel;
    public string superStructureLayerLevelText;
    public string superStructureLayerLevelTextSimple;
    public int resistentArmorLayerEvol;
    public string resistentArmorLayerEvolText;
    public int interiorArmorLayerEvol;
    public string interiorArmorLayerEvolText;
    public int superStructureLayerEvol;
    public string superStructureLayerEvolText;

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

        resistentArmorLayerEvol = FormulaEvolucao(lastInspection.ResistentArmorLayer.CalculateDamageLevel(), referenceInspection.ResistentArmorLayer.CalculateDamageLevel());
        resistentArmorLayerLevel = FormulaRisco(lastInspection.ResistentArmorLayer.CalculateDamageLevel(), resistentArmorLayerEvol);
        
        interiorArmorLayerEvol = FormulaEvolucao(lastInspection.InteriorArmorLayer.CalculateDamageLevel(), referenceInspection.InteriorArmorLayer.CalculateDamageLevel());
        interiorArmorLayerLevel = FormulaRisco(lastInspection.InteriorArmorLayer.CalculateDamageLevel(), interiorArmorLayerEvol);

        superStructureLayerEvol = FormulaEvolucao(lastInspection.Superstructure.CalculateDamageLevel(), referenceInspection.Superstructure.CalculateDamageLevel());
        superStructureLayerLevel = FormulaRisco(lastInspection.Superstructure.CalculateDamageLevel(), superStructureLayerEvol);
        
        riskLevel = resistentArmorLayerLevel;

        if (interiorArmorLayerLevel > riskLevel)
            riskLevel = interiorArmorLayerLevel;

        if (superStructureLayerLevel > riskLevel)
            riskLevel = superStructureLayerLevel;
        
        PrepareLevelTexts();
    }

    public void PrepareLevelTexts()
    {
        string[] descriptionSimple = {"Sem risco aparente",
            "Baixo risco (observação atenta)",
            "Risco moderado (reparação aconselhável)",
            "Alto risco (reparação urgente)",
            "Destruição"};

        string[] description = {"Sem risco aparente",
            "Baixo",
            "Moderado",
            "Alto",
            "Destruição"};

        string[] evolDescription = {"Não se detectou qualquer evolução; as condições permanecem inalteráveis",
            "Evolução muito ligeira; pode ser considerada insignificante",
            "Evolução ligeira; Processa-se a velocidade reduzida, mas existe e é visível",
            "Evolução acentuada; muitas diferenças relativamente a observações anteriores",
            "Evolução muito acentuada; diferenças significativas relativamente a observações anteriores",
            "Foi atingida a destruição do elememto"};

        
        resistentArmorLayerLevelText = description[resistentArmorLayerLevel];
        resistentArmorLayerLevelTextSimple = descriptionSimple[resistentArmorLayerLevel];
        resistentArmorLayerEvolText = evolDescription[resistentArmorLayerEvol];

        interiorArmorLayerLevelText = description[interiorArmorLayerLevel];
        interiorArmorLayerLevelTextSimple = descriptionSimple[interiorArmorLayerLevel];
        interiorArmorLayerEvolText = evolDescription[interiorArmorLayerEvol];

        superStructureLayerLevelText = description[superStructureLayerLevel];
        superStructureLayerLevelTextSimple = descriptionSimple[superStructureLayerLevel];
        superStructureLayerEvolText = evolDescription[superStructureLayerEvol];
    }

    public Dictionary<string, string> GetUIData()
    {
        return new Dictionary<string, string>
        {
            { "Title", $"Portimão Poente ({name})" },
            { "Manto", $"Manto Resistente: {resistentArmorLayerLevelTextSimple}" },
            { "Tardoz", $"Tardoz: {interiorArmorLayerLevelTextSimple}" },
            { "Coroamento", $"Coroamento: {superStructureLayerLevelTextSimple}" },
            { "LastInspection", $"Última inspeção: {lastInspection?.Year}" },
            { "ReferenceInspection", $"Inspeção de Referência: {referenceInspection?.Year}" }
        };
    }

    public Dictionary<string, LayerUIData> GetRiskLevelUIData()
    {
        return new Dictionary<string, LayerUIData>
        {
            ["Manto"] = new LayerUIData
            {
                Level = $"Manto Resistente [Grau {resistentArmorLayerLevel}]: {resistentArmorLayerLevelText}",
                Evol = $"Manto Resistente [Grau {resistentArmorLayerEvol}]: {resistentArmorLayerEvolText}",
                RefState = $"Manto Resistente {referenceInspection.ResistentArmorLayer.getLevelString()} [Grau {referenceInspection.ResistentArmorLayer.DamageLevel}]",
                LastState = $"Manto Resistente {lastInspection.ResistentArmorLayer.getLevelString()} [Grau {lastInspection.ResistentArmorLayer.DamageLevel}]"
            },

            ["Coroamento"] = new LayerUIData
            {
                Level = $"Coroamento [Grau {superStructureLayerLevel}]: {superStructureLayerLevelText}",
                Evol = $"Coroamento [Grau {superStructureLayerEvol}]: {superStructureLayerEvolText}",
                RefState = $"Coroamento {referenceInspection.Superstructure.getLevelString()} [Grau {referenceInspection.Superstructure.DamageLevel}]",
                LastState = $"Coroamento {lastInspection.Superstructure.getLevelString()} [Grau {lastInspection.Superstructure.DamageLevel}]"
            },

            ["Tardoz"] = new LayerUIData
            {
                Level = $"Tardoz [Grau {interiorArmorLayerLevel}]: {interiorArmorLayerLevelText}",
                Evol = $"Tardoz [Grau {interiorArmorLayerEvol}]: {interiorArmorLayerEvolText}",
                RefState = $"Tardoz {referenceInspection.InteriorArmorLayer.getLevelString()} [Grau {referenceInspection.InteriorArmorLayer.DamageLevel}]",
                LastState = $"Tardoz {lastInspection.InteriorArmorLayer.getLevelString()} [Grau {lastInspection.InteriorArmorLayer.DamageLevel}]"
            }
        };
    }

    public Inspection GetInspectionFromYear(int year)
    {
        foreach (Inspection insp in Inspections)
        {
            if (insp.Year == year)
            {
                return insp;
            }
        }

        //in case year not found
        return lastInspection;
    }
}
