using UnityEngine;

[System.Serializable]
public class InteriorArmorLayerInspection
{
    public string Falls;

    public string FallsText;

    public string Fractures;

    public string FracturesText;

    public string Talude;

    public string TaludeText;

    public string Quantity;

    public string QuantityText;

    public string Description;

    public string DescriptionText;

    public string Sound;

    public string SoundText;

    //Assentamento do Manto
    public string NearWaterLine;

    public string Coroamento;

    public string MaiorAssentamento;

    public string MaiorAssentamentoText;

    public string Observations;

    public string GeneralOpinion;
    public string GeneralOpinionText;

    public int DamageLevel;
    public string DamageLevelText;

    public int CalculateDamageLevel()
    {
        double val = 
        (5 * int.Parse(Falls)
		+	4 * int.Parse(Fractures)
		+	3 * int.Parse(Talude)
		+	int.Parse(Quantity)
		+	int.Parse(Description)
		+	int.Parse(Sound)
		+	2 * int.Parse(MaiorAssentamento)
		+   0.06 * int.Parse(GeneralOpinion)
		) / 23;

        DamageLevel = Enquadra(val);
        PrepareTexts();
        return DamageLevel;
    }

    private int Enquadra(double level)
    {
        double[] numEnquadra = { 0.7, 1.0, 1.7, 2.3, 2.77 };

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
        if (!int.TryParse(value , out int i))
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
        FallsText = GetTextWithLabel(Falls, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        FracturesText = GetTextWithLabel(Fractures, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        TaludeText = GetTextWithLabel(Talude, new[]
        {
            "Bom estado",
            "Degradado junto à linha de água",
            "Degradado",
            "Muito degradado"
        });

        QuantityText = GetTextWithLabel(Quantity, new[]
        {
            "Em bom estado",
            "Bom mas com muitos poros superficiais",
            "Alguma corrosão",
            "Muita corrosão"
        });

        DescriptionText = GetTextWithLabel(Description, new[]
        {
            "Cantos intactos",
            "Cantos arredondados"
        });

        SoundText = GetTextWithLabel(Sound, new[]
        {
            "Sólido",
            "Oco"
        });

        MaiorAssentamentoText = GetTextWithLabel(MaiorAssentamento, new[]
        {
            "Não há assentamento",
            "Assentamento ≤ 0.5m",
            "0.5m < Assentamento ≤ 1.0m",
            "Assentamento > 1.0m"
        });

        GeneralOpinionText = GetTextWithThreshold(GeneralOpinion, new[]
        {
            (35,  "Grau 0"),
            (85,  "Grau 1"),
            (135, "Grau 2"),
            (200, "Grau 3"),
            (255, "Grau 4"),
            (280, "Grau 5")
        });

        DamageLevelText = GetTextWithLabel(DamageLevel.ToString(), new[]
        {
            "Grau 0: Tardoz em bom estado",
            "Grau 1: Tardoz em bom estado mas com sinais pontuais de degradação ligeira",
            "Grau 2: Tardoz ligeiramente degradado", 
            "Grau 3: Tardoz degradado",		
            "Grau 4: Tardoz muito degradado",	
            "Grau 5: Tardoz em ruína"
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
