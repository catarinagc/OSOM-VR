using UnityEngine;
using System.Collections.Generic;
public class FoundationDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "FundacaoCota", "Z1" },
        // { "", "Z11" },
        // { "", "L5" },
        { "InclinacaoF", "Inclinacao" },
        {"TipoF", "Type" },
        {"PesoF", "Weight" },
        {"DisposF", "Disposicao" },
        {"NaturezaF", "Nature" },
        {"PesoEspF", "SpecificWeight" }
    };
}
