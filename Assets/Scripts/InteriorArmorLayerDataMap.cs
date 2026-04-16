using UnityEngine;
using System.Collections.Generic;
public class InteriorArmorLayerDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "InclinacaoT", "I2" },
        // { "", "I2a" },
        { "CotaSuperiorTardoz", "Z7" },
        { "CotaInferiorTardoz", "Z8" },
        //{ "", "Z13" },
        {"TipoT", "Type1" },
        {"PesoT", "Weight1" },
        {"DisposT", "Disposicao1" },
        {"NaturezaT", "Nature1" },
        {"PesoEspT", "SpecificWeight1" }
        // {"", "Type2" },
        // {"", "Weight2" },
        // {"", "Disposicao2" },
        // {"", "Nature2" },
        // {"", "SpecificWeight2" }
    };
}
