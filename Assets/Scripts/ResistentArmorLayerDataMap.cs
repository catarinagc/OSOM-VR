using UnityEngine;
using System.Collections.Generic;
public class ResistentArmorLayerDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "Inclinacao", "I1" },
        { "InclinacaoSub", "I1a" },
        { "CotaSuperior", "Z4" },
        { "CotaInferior", "Z3" },
        { "CotaIntM", "Z12" },
        {"TipoM", "Type1" },
        {"PesoM", "Weight1" },
        {"DisposM", "Disposicao1" },
        {"NaturezaM", "Nature1" },
        {"PesoEspM", "SpecificWeight1" },
        {"TipoMSub", "Type2" },
        {"PesoMSub", "Weight2" },
        {"DisposMSub", "Disposicao2" },
        {"NaturezaMSub", "Nature2" },
        {"PesoEspMSub", "SpecificWeight2" }
    };
}
