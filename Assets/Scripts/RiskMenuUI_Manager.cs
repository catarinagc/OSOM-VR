using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class RiskMenuUI_Manager : MonoBehaviour
{
    [System.Serializable]
    public class RiskLevelUIGroup
    {
        public string key;
        public TMP_Text level;
        public TMP_Text evol;
        public TMP_Text refState;
        public TMP_Text lastState;
    }

    [SerializeField] TMP_Text title_text;
    [SerializeField] TMP_Text last_state_text;
    [SerializeField] TMP_Text ref_state_text;
    [SerializeField] TMP_Text evol_text;
    [SerializeField] TMP_Text risk_text;
    
    [SerializeField] List<RiskLevelUIGroup> riskTexts;

    private string default_title_text;
    
    void Awake()
    {
        default_title_text = "Portimão Poente ";
    }
    
    public void PrepareOpen(Zone zone)
    {
        title_text.text = $"{default_title_text} ({zone.name})";
        last_state_text.text = "Estado em " + zone.lastInspection.Year;
        ref_state_text.text = "Estado em " + zone.referenceInspection.Year;
        evol_text.text = "Evolução de " + zone.referenceInspection.Year + " a " + zone.lastInspection.Year;
        risk_text.text = "Risco em " + zone.lastInspection.Year;

        var data = zone.GetRiskLevelUIData();

        foreach (var txt in riskTexts)
        {
            if (!data.ContainsKey(txt.key)) continue;

            var d = data[txt.key];

            txt.level.text = d.Level;
            txt.evol.text = d.Evol;
            txt.refState.text = d.RefState;
            txt.lastState.text = d.LastState;
        }
    }
}
