using UnityEngine;

[System.Serializable]
public class InteriorArmorLayerInspection
{
    public string Falls;

    public string Fractures;

    public string Talude;

    public string Quantity;

    public string Description;

    public string Sound;

    //Assentamento do Manto
    public string NearWaterLine;

    public string Coroamento;

    public string MaiorAssentamento;

    public string Observations;

    public string GeneralOpinion;

    public int DamageLevel;

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

        return Enquadra(val);
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

    // var niveis = {
	// M: [0.6, 0.9, 1.4, 2.0, 2.6],
	// S: [0.1, 0.3, 0.8, 1.3, 1.8],
	// T: [0.7, 1.0, 1.7, 2.3, 2.77]
    // };

}
