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
    }
}
