using UnityEngine;
using TMPro;
public class zoneUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text title_text;
    private string default_title_text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        default_title_text = "Portimão Poente ";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrepareOpen(string zoneSelected)
    {
        title_text.text = default_title_text + "(" + zoneSelected + ")";
    }
}
