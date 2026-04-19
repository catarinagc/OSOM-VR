using UnityEngine;
using System.Collections.Generic;
public class UnderwaterInspectionMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "QuedasMSub", "ArmorLayerFalls" },
        { "FracturasMSub", "ArmorLayerFractures" },
        { "TaludeMSub", "ArmorLayerTalude" },
        { "DSM_QuantidadeMSub", "ArmorLayerQuantity" },
        {"QuedasF", "TaludeFalls" },
        {"FracturasF", "TaludeFractures" },
        {"TaludeF", "TaludeTalude" },
        {"DSM_QuantidadeF", "TaludeQuantity" },
        {"QuedasTSub", "TardozFalls" },
        {"FracturasTSub", "TardozFractures" },
        {"TaludeTSub", "TardozTalude" },
        {"DSM_QuantidadeTSub", "TardozQuantity" },
        {"ObservacoesSub", "Observations" },
        {"OpiniaoSub", "GeneralOpinion" }
    };
}
