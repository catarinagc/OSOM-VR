using UnityEngine;

[System.Serializable]
public class PierInspection
{
    //Bordo do Cais
    public string PierFracture;

    public string PierFractureText;

    public string PierConcrete;

    public string PierConcreteText;

    public string Assentamento;

    public string Deslizamento;

    public string Rotation;

    //Platform
    public string PlatformFracture;

    public string PlatformFractureText;

    public string PlatformConcrete;

    public string PlatformConcreteText;

    public string ObstrucaoOrificios;

    public string ObstrucaoOrificiosText;

    public string Deslocamento;

    public string DeslocamentoText;

    public string Observations;

    public string GeneralOpinion;


    void Start()
    {
        PrepareTexts();
    }
    
    private int GetSafeIndex(string value, int length)
    {
        if (!int.TryParse(value , out int i))
            return 0;

        if (i < 0 || i >= length)
            return 0;

        return i;
    }

    private string GetTextWithLabel(string value, string[] map)
    {
        int i = GetSafeIndex(value, map.Length);

        return $"{map[i]}";
    }

    public void PrepareTexts()
    {

        PierFractureText = GetTextWithLabel(PierFracture, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        PierConcreteText = GetTextWithLabel(PierConcrete, new[]
        {
            "Bom estado",
            "Pontualmente degradado",
            "Degradado",
            "Muito degradado"
        });

        PlatformFractureText = GetTextWithLabel(PlatformFracture, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        PlatformConcreteText = GetTextWithLabel(PlatformConcrete, new[]
        {
            "Bom estado",
            "Pontualmente degradado",
            "Degradado",
            "Muito degradado"
        });

        ObstrucaoOrificiosText = GetTextWithLabel(ObstrucaoOrificios, new[]
        {
            "≤ 50%",
            "> 50%"
        });

        DeslocamentoText = GetTextWithLabel(Deslocamento, new[]
        {
            "Sem abertura",
            "Abertura ≤ 2cm",
            "2cm < Abertura ≤ 5cm",
            "Abertura > 5cm"
        });
    }
}
