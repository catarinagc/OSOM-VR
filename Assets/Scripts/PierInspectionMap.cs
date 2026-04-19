using UnityEngine;
using System.Collections.Generic;
public class PierInspectionMap
{
    public static readonly Dictionary<string, string> Map = new()
    {
        { "FracturasBordoC", "PierFracture" },
        { "DegBetao_BordoC", "PierConcrete" },
        { "AssentamentoBordoC", "Assentamento" },
        { "DeslizamentoBordoC", "Deslizamento" },
        {"RotacaoBordoC", "Rotation" },
        {"FracturasPlataformaC", "PlatformFracture" },
        {"DegBetao_PlataformaC", "PlatformConcrete" },
        {"OrificiosPlataformaC", "ObstrucaoOrificios" },
        {"DeslocJuntasPlataformaC", "Deslocamento" },
        {"ObservacoesC", "Observations" },
        {"OpiniaoC", "GeneralOpinion" }
    };
}
