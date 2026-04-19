using UnityEngine;
using System.Collections.Generic;
public class InteriorArmorLayerDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "InclinacaoT", "I2" },
        { "InclinacaoTSub", "I2a" },
        { "CotaSuperiorTardoz", "Z7" },
        { "CotaInferiorTardoz", "Z8" },
        { "CotaIntT", "Z13" },
        {"TipoT", "Type1" },
        {"PesoT", "Weight1" },
        {"DisposT", "Disposicao1" },
        {"NaturezaT", "Nature1" },
        {"PesoEspT", "SpecificWeight1" },
        {"TipoTSub", "Type2" },
        {"PesoTSub", "Weight2" },
        {"DisposTSub", "Disposicao2" },
        {"NaturezaTSub", "Nature2" },
        {"PesoEspTSub", "SpecificWeight2" }
    };
}
