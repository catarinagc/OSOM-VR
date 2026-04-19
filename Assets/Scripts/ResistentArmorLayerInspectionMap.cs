using UnityEngine;
using System.Collections.Generic;
public class ResistentArmorLayerInspectionMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "QuedasM", "Falls" },
        { "FracturasM", "Fractures" },
        { "TaludeM", "Talude" },
        { "DSM_QuantidadeM", "Quantity" },
        { "DSM_DescricaoM", "Description" },
        { "DSM_SomM", "Sound" },
        { "AM_LinhaÁguaM", "NearWaterLine" },
        { "AM_CoroamentoM", "Coroamento" },
        { "AM_MaiorAssentamentoM", "MaiorAssentamento" },
        { "ObservacoesM", "Observation" },
        { "OpiniaoM", "GeneralOpinion" }
    };
}
