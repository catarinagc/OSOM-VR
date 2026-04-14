using UnityEngine;
using System.Collections.Generic;
public class OuterCrestBermDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "CotaBC", "Z4" },
        { "LarguraBC", "L3" },
        { "TipoBermaExt", "Type" },
        { "PesoBermaExt", "Weight" },
        { "DisposBermaExt", "Disposicao" },
        {"NaturezaBermaExt", "Nature" },
        {"PesoEspBermaExt", "SpecificWeight" }
    };
}
