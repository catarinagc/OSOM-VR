using UnityEngine;
using System.Collections.Generic;
public class ToeBermDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "InclinacaoP", "I3" },
        { "CotaSuperiorPe", "Z2" },
        { "CotaInferiorPe", "Z1" },
        { "LarguraP", "L4" },
        {"TipoBanqPeTalude", "Type" },
        {"PesoBanqPeTalude", "Weight" },
        {"DisposBanqPeTalude", "Disposicao" },
        {"NaturezaBanqPeTalude", "Nature" },
        {"PesoEspBanqPeTalude", "SpecificWeight" }
    };
}
