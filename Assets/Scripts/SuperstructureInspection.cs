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

    public string GeneralOpinionText;

    public int DamageLevel;

    public string DamageLevelText;

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
            "Em bom estado",
            "Em bom estado mas com sinais pontuais de degradação ligeira",
            "Ligeiramente degradado", 
            "Degradado",			
            "Muito degradado",		
            "Em ruína"
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
        
        GeneralOpinionText = GetTextWithThreshold(GeneralOpinion, new[]
        {
            (5,  "Grau 0"),
            (20,  "Grau 1"),
            (55, "Grau 2"),
            (105, "Grau 3"),
            (155, "Grau 4"),
            (190, "Grau 5")
        });

        DamageLevelText = GetTextWithLabel(DamageLevel.ToString(), new[]
        {
            "Grau 0: Coroamento em bom estado",
            "Grau 1: Coroamento em bom estado mas com sinais pontuais de degradação ligeira",
            "Grau 2: Coroamento ligeiramente degradado", 
            "Grau 3: Coroamento degradado",		
            "Grau 4: Coroamento muito degradado",	
            "Grau 5: Coroamento em ruína"
        });

    }

    private string GetTextWithThreshold(string value, (int threshold, string label)[] map)
    {
        if (!int.TryParse(value, out int val))
            return "-";

        foreach (var (threshold, label) in map)
        {
            if (val <= threshold)
                return label;
        }

        return map[map.Length - 1].label; // fallback to highest grade
    }
}
