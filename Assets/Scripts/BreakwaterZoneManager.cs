using UnityEngine;

public class BreakwaterZoneManager : MonoBehaviour
{

    [SerializeField] float minTest;
    [SerializeField] float maxTest;
    [SerializeField] bool isActive;
    [SerializeField] GameObject breakwater;
    [SerializeField] Material clipMaterial;
    public int totalAmountZones;
    //array de arrays a guardar max e min de cada zona
    //ter menu de escolha da zona, geral, A, B, ....

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            clipMaterial.SetFloat("_min", minTest);
            clipMaterial.SetFloat("_max", maxTest);
        }
        else
        {
            clipMaterial.SetFloat("_min", -60);
            clipMaterial.SetFloat("_max", 60);
        }
    }
}
