using UnityEngine;

[System.Serializable]
public class ResistentArmorLayerInspection
{
    public string Falls;
    
    public string Fractures;
    
    public string Talude;
    
    public string Quantity;
    
    public string Description;
    
    public string Sound;
    
    public string NearWaterLine;
    
    public string Coroamento;
    
    public string MaiorAssentamento;
    
    public string Observation;
    
    public string GeneralOpinion;
    
    public int DamageLevel;

    public int CalculateDamageLevel()
    {
        double val = (5 * int.Parse(Falls)
		+ 4 * int.Parse(Fractures)
		+ 2 * int.Parse(Talude)
		+ int.Parse(Quantity)
		+ int.Parse(Description)
		+ int.Parse(Sound)
		+ 3 * int.Parse(MaiorAssentamento)
		+ 0.06 * int.Parse(GeneralOpinion)
		) / 23;

        return Enquadra(val);
    }

    private int Enquadra(double level)
    {
        double[] numEnquadra = {0.6, 0.9, 1.4, 2.0, 2.6};

        for (int i = 0; i < numEnquadra.Length; i++)
        {
            if (level <= numEnquadra[i])
                return i;
        }

        // fallback if level is higher than all values
        return numEnquadra.Length - 1;
    }
}
