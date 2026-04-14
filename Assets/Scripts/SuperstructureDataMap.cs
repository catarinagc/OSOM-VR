using UnityEngine;
using System.Collections.Generic;
public class SuperstructureDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "Tipo", "Type" },
        { "CotadeFundacao", "Z5" },
        { "CotadePIMC", "Z6" },
        { "LarguradeCoroamento", "L2" },
        { "CotadeCoroamento", "Z10" },
        {"Deflector", "Deflector" },
        {"LarguraPass", "CoroamentoLength" },
        {"CotaDente", "CotaFundacaoDente" },
        {"CotaPass", "CotaPasseio" },
        {"TipoC", "MaterialType" },
        {"PesoC", "Weight" },
        {"DisposC", "Disposicao" },
        {"NaturezaC", "Nature" },
        {"PesoEspC", "SpecificWeight" }
    };
}
