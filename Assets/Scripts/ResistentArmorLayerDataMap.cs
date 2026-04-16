using UnityEngine;
using System.Collections.Generic;
public class ResistentArmorLayerDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "InclinacaoF", "I1" },
        //{ "", "I1a" },
        { "CotaSuperior", "Z4" },
        { "CotaInferior", "Z3" },
        // { "", "Z12" },
        {"TipoM", "Type1" },
        {"PesoM", "Weight1" },
        {"DisposM", "Disposicao1" },
        {"NaturezaM", "Nature1" },
        {"PesoEspM", "SpecificWeight1" }
    };
}
