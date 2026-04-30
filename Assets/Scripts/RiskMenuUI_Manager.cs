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

    // //last state
    // [SerializeField] TMP_Text manto_last_state;
    // [SerializeField] TMP_Text coroamento_last_state;
    // [SerializeField] TMP_Text tardoz_last_state;

    // //ref state
    // [SerializeField] TMP_Text manto_ref_state;
    // [SerializeField] TMP_Text coroamento_ref_state;
    // [SerializeField] TMP_Text tardoz_ref_state;

    // //evolucao
    // [SerializeField] TMP_Text manto_evol;
    // [SerializeField] TMP_Text coroamento_evol;
    // [SerializeField] TMP_Text tardoz_evol;

    // //risk level
    // [SerializeField] TMP_Text manto_level;
    // [SerializeField] TMP_Text coroamento_level;
    // [SerializeField] TMP_Text tardoz_level;
    
    [SerializeField] List<RiskLevelUIGroup> riskTexts;

    private string default_title_text;
    
    void Awake()
    {
        default_title_text = "Portimão Poente ";
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // public void PrepareOpen(Zone zone)
    // {
    //     title_text.text = default_title_text + "(" + zone.name + ")";
    //     //risk level
    //     manto_level.text = "Manto Resistente [Grau " + zone.resistentArmorLayerLevel + "]: " + zone.resistentArmorLayerLevelText;
    //     coroamento_level.text = "Coroamento [Grau " + zone.superStructureLayerLevel + "]: " + zone.superStructureLayerLevelText;
    //     tardoz_level.text = "Tardoz [Grau " + zone.interiorArmorLayerLevel + "]: " + zone.interiorArmorLayerLevelText;
    //     //evolDescription
    //     manto_evol.text = "Manto Resistente [Grau " + zone.resistentArmorLayerEvol + "]: " + zone.resistentArmorLayerEvolText;
    //     coroamento_evol.text = "Coroamento [Grau " + zone.superStructureLayerEvol + "]: " + zone.superStructureLayerEvolText;
    //     tardoz_evol.text = "Tardoz [Grau " + zone.interiorArmorLayerLevel + "]: " + zone.interiorArmorLayerEvolText;
    //     //estado ref
    //     manto_ref_state.text = "Manto Resistente " + zone.referenceInspection.ResistentArmorLayer.getLevelString() + " [Grau " + zone.referenceInspection.ResistentArmorLayer.DamageLevel + " ]";
    //     coroamento_ref_state.text = "Coroamento " + zone.referenceInspection.Superstructure.getLevelString() + " [Grau " + zone.referenceInspection.Superstructure.DamageLevel + " ]";
    //     tardoz_ref_state.text = "Tardoz " + zone.referenceInspection.InteriorArmorLayer.getLevelString() + " [Grau " + zone.referenceInspection.InteriorArmorLayer.DamageLevel + " ]";
    //     //estado estadoAtual
    //     manto_last_state.text = "Manto Resistente " + zone.lastInspection.ResistentArmorLayer.getLevelString() + " [Grau " + zone.lastInspection.ResistentArmorLayer.DamageLevel + " ]";
    //     coroamento_last_state.text = "Coroamento " + zone.lastInspection.Superstructure.getLevelString() + " [Grau " + zone.lastInspection.Superstructure.DamageLevel + " ]";
    //     tardoz_last_state.text = "Tardoz " + zone.lastInspection.InteriorArmorLayer.getLevelString() + " [Grau " + zone.lastInspection.InteriorArmorLayer.DamageLevel + " ]";
    // }

    public void PrepareOpen(Zone zone)
    {
        title_text.text = $"{default_title_text} ({zone.Id})";

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
