using UnityEngine;
using System.Collections.Generic;
public static class GeneralZoneDataMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        // { "" , "Year"},
        { "CoordenadaM1" , "X1"},
        { "CoordenadaM2" , "X2"},
        { "CoordenadaP1" , "Y1"},
        { "CoordenadaP2" , "Y2"},
        { "Zona", "Zone" },
        { "Comprimento", "Length" },
        { "Largura", "Width" },
        { "ProfundidadeMaxima", "MaxDepth" },
        { "ProfundidadeMinima", "MinDepth" }
    };
}
