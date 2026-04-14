using UnityEngine;
using System.Collections.Generic;
public static class GeneralZoneDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "Comprimento", "Length" },
        { "Largura", "Width" },
        { "ProfundidadeMaxima", "MaxDepth" },
        { "ProfundidadeMinima", "MinDepth" },
        { "Zona", "Zone" }
    };
}
