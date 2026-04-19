using UnityEngine;

[System.Serializable]
public class SuperstructureInspection
{
    public string Fractures;

    public string Quantity;

    public string Description;

    public string Assentamento;

    public string Derrubamento;

    public string Deslizamento;

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

        return Enquadra(val);
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
}
