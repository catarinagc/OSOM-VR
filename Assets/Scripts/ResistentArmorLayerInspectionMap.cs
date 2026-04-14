using UnityEngine;
using System.Collections.Generic;
public class ResistentArmorLayerInspectionMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "QuedasM", "Falls" },
        { "FracturasM", "Fractures" },
        { "TaludeM", "Talude" },
        { "", "Quantity" },
        { "", "Description" },
        { "", "Sound" },
        { "", "NearWaterLine" },
        { "", "Coroamento" },
        { "", "MaiorAssentamento" },
        { "", "Observation" },
        { "", "GeneralOpinion" },
        { "", "DamageLevel" }
    };
}
