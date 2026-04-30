using UnityEngine;

[System.Serializable]
public class SuperstructureInspection
{
    public string Fractures;

    public string FracturesText;

    public string Quantity;

    public string QuantityText;

    public string Description;

    public string Assentamento;

    public string AssentamentoText;

    public string Derrubamento;

    public string DerrubamentoText;

    public string Deslizamento;

    public string DeslizamentoText;

    public string Observations;

    public string GeneralOpinion;

    public int DamageLevel;

    public int CalculateDamageLevel()
    {
        double val = (2 * int.Parse(Fractures)
		+	2 * int.Parse(Quantity)
		+	3 * int.Parse(Assentamento)
		+	4 * int.Parse(Derrubamento)
		+	4 * int.Parse(Deslizamento)
		+	0.05 * int.Parse(GeneralOpinion)
		) / 20;

        DamageLevel = Enquadra(val);
        PrepareTexts();
        return DamageLevel;
    }

    private int Enquadra(double level)
    {
        double[] numEnquadra = {0.1, 0.3, 0.8, 1.3, 1.8};

        for (int i = 0; i < numEnquadra.Length; i++)
        {
            if (level <= numEnquadra[i])
                return i;
        }

        // fallback if level is higher than all values
        return numEnquadra.Length - 1;
    }

    public string getLevelString()
    {
        string[] stateDescription =
        {
            "em bom estado",
            "em bom estado mas com sinais pontuais de degradação ligeira",
            "ligeiramente degradado", 
            "degradado",			
            "muito degradado",		
            "em ruína"
        };

        return stateDescription[DamageLevel];
    }

    private int GetSafeIndex(string value, int length)
    {
        if (!int.TryParse(value, out int i))
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
        FracturesText = GetTextWithLabel(Fractures, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        QuantityText = GetTextWithLabel(Quantity, new[]
        {
            "Em bom estado",
            "Alguma corrosão",
            "Muita corrosão"
        });

        AssentamentoText = GetTextWithLabel(Assentamento, new[]
        {
            "Não há assentamento",
            "Assentamento ≤ 0.5m",
            "Assentamento > 0.5m"
        });

        DerrubamentoText = GetTextWithLabel(Derrubamento, new[]
        {
            "Não há derrubamento",
            "Derrubamento ≤ 0.5m",
            "Derrubamento > 0.5m"
        });

        DeslizamentoText = GetTextWithLabel(Deslizamento, new[]
        {
            "Não há deslizamento",
            "Deslizamento ≤ 0.5m",
            "Deslizamento > 0.5m"
        });

    }
}
