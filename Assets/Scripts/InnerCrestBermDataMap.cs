using UnityEngine;
using System.Collections.Generic;
public class InnerCrestBermDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "CotaBCI", "Z7" },
        { "LarguraBCI", "L1" },
        { "TipoBermaInt", "MaterialType" },
        { "PesoBermaInt", "MaterialWeight" },
        { "DisposBermaInt", "MaterialDisposicao" },
        {"NaturezaBermaInt", "MaterialNature" },
        {"PesoEspBermaInt", "MaterialSpecificWeight" }
    };
}
