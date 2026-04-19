using UnityEngine;
using System.Collections.Generic;
public class InteriorArmorLayerInspectionMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
            { "QuedasT", "Falls" },
            { "FracturasT", "Fractures" },
            { "TaludeT", "Talude" },
            { "DSM_QuantidadeT", "Quantity" },
            {"DSM_DescricaoT", "Description" },
            {"DSM_SomT", "Sound" },
            {"AM_LinhaAguaT", "NearWaterLine" },
            {"AM_CoroamentoT", "Coroamento" },
            {"AM_MaiorAssentamentoT", "MaiorAssentamento" },
            {"ObservacoesT", "Observations" },
            {"OpiniaoT", "GeneralOpinion" }
    };
}
