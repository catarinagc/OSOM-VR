using UnityEngine;

[System.Serializable]
public class UnderwaterInspection
{
    //Manto Resistente
    public string ArmorLayerFalls;

    public string ArmorLayerFallsText;

    public string ArmorLayerFractures;

    public string ArmorLayerFracturesText;

    public string ArmorLayerTalude;

    public string ArmorLayerTaludeText;

    public string ArmorLayerQuantity;

    public string ArmorLayerQuantityText;

    public string TaludeFalls;

    public string TaludeFallsText;
    
    public string TaludeFractures;

    public string TaludeFracturesText;

    public string TaludeTalude;

    public string TaludeTaludeText;

    public string TaludeQuantity;

    public string TaludeQuantityText;

    //Tardoz
    public string TardozFalls;

    public string TardozFallsText;

    public string TardozFractures;

    public string TardozFracturesText;

    public string TardozTalude;

     public string TardozTaludeText;

    public string TardozQuantity;

    public string TardozQuantityText;

    public string Observations;

    public string GeneralOpinion;

    public string GeneralOpinionText;


    void Start()
    {
        PrepareTexts();
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
        // --- Manto Resistente ---
        ArmorLayerFallsText = GetTextWithLabel(ArmorLayerFalls, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        ArmorLayerFracturesText = GetTextWithLabel(ArmorLayerFractures, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        ArmorLayerTaludeText = GetTextWithLabel(ArmorLayerTalude, new[]
        {
            "Bom estado",
            "Pouco degradado",
            "Degradado",
            "Muito degradado"
        });

        ArmorLayerQuantityText = GetTextWithLabel(ArmorLayerQuantity, new[]
        {
            "Em bom estado",
            "Bom mas com muitos poros superficiais",
            "Alguma corrosão",
            "Muita corrosão"
        });

        TaludeFallsText = GetTextWithLabel(TaludeFalls, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        TaludeFracturesText = GetTextWithLabel(TaludeFractures, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        TaludeTaludeText = GetTextWithLabel(TaludeTalude, new[]
        {
            "Bom estado",
            "Pouco degradado",
            "Degradado",
            "Muito degradado"
        });

        TaludeQuantityText = GetTextWithLabel(TaludeQuantity, new[]
        {
            "Em bom estado",
            "Bom mas com muitos poros superficiais",
            "Alguma corrosão",
            "Muita corrosão"
        });

        // --- Tardoz ---
        TardozFallsText = GetTextWithLabel(TardozFalls, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        TardozFracturesText = GetTextWithLabel(TardozFractures, new[]
        {
            "Nenhumas",
            "Poucas",
            "Significativas",
            "Muitas"
        });

        TardozTaludeText = GetTextWithLabel(TardozTalude, new[]
        {
            "Bom estado",
            "Pouco degradado",
            "Degradado",
            "Muito degradado"
        });

        TardozQuantityText = GetTextWithLabel(TardozQuantity, new[]
        {
            "Em bom estado",
            "Bom mas com muitos poros superficiais",
            "Alguma corrosão",
            "Muita corrosão"
        });

        GeneralOpinionText = GetTextWithThreshold(GeneralOpinion, new[]
        {
            (30,  "Grau 0"),
            (75,  "Grau 1"),
            (115, "Grau 2"),
            (170, "Grau 3"),
            (230, "Grau 4"),
            (275, "Grau 5")
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
