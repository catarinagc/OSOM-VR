using UnityEngine;
using System.Collections.Generic;
public class SuperstructureInspectionMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "FracturasS", "Fractures" },
        { "DSM_QuantidadeS", "Quantity" },
        { "DSM_DescricaoS", "Description" },
        { "AssentamentosS", "Assentamento" },
        { "DerrubamentosS", "Derrubamento" },
        { "DeslizamentosS", "Deslizamento" },
        { "ObservacoesS", "Observations" },
        { "OpiniaoS", "GeneralOpinion" }
    };
}
