using UnityEngine;
using TMPro;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
public class ZoneInfoUI_Manager : MonoBehaviour
{
    [SerializeField] TMP_Text title_text;

    [System.Serializable]
    public class UIField
    {
        public string fieldName;
        public TMP_Text labelText;
        public TMP_Text text;
    }

    [SerializeField] List<UIField> generalFields;
    [SerializeField] List<UIField> superstructureFields;
    [SerializeField] List<UIField> innerCrestFields;
    [SerializeField] List<UIField> interiorArmorFields;
    [SerializeField] List<UIField> outerCrestFields;
    [SerializeField] List<UIField> resistentArmorFields;
    [SerializeField] List<UIField> toeBermFields;
    [SerializeField] List<UIField> foundationFields;
    
    [SerializeField] UI_Manager uI_Manager;

    void BindObjectToUI(object data, List<UIField> fields, string[] labels)
    {
        var type = data.GetType();

        for (int i = 0; i < fields.Count; i++)
        {
            var fieldUI = fields[i];

            // get label from your array
            string label = i < labels.Length ? labels[i] : fieldUI.fieldName;

            var field = type.GetField(fieldUI.fieldName);

            string valueStr = "-";

            if (field != null)
            {
                var value = field.GetValue(data);
                valueStr = value != null ? value.ToString() : "-";
            }

            //fieldUI.text.text = $"{label}: {valueStr}";
            fieldUI.labelText.text = label;
            fieldUI.text.text = valueStr;
        }
    }

    private string default_title_text;
    
    void Awake()
    {
        default_title_text = "Portimão Poente ";
    }

    public void CloseUI()
    {
        uI_Manager.CloseSpecificUI(this.gameObject);
    }

    public void OnMenuClicked()
    {
        uI_Manager.ReopenSelectorForMenu(this.gameObject);
    }

    public void PrepareOpen(Zone zone)
    {
        title_text.text = default_title_text + "(" + zone.name + ")";

        BindObjectToUI(zone.Caracteristics.General, generalFields, infoGeral);
        BindObjectToUI(zone.Caracteristics.Superstructure, superstructureFields, infoC);
        BindObjectToUI(zone.Caracteristics.InnerCrestBerm, innerCrestFields, infoBCI);
        BindObjectToUI(zone.Caracteristics.InteriorArmorLayer, interiorArmorFields, infoT);
        BindObjectToUI(zone.Caracteristics.OuterCrestBerm, outerCrestFields, infoBC);
        BindObjectToUI(zone.Caracteristics.ResistentArmorLayer, resistentArmorFields, infoMR);
        BindObjectToUI(zone.Caracteristics.ToeBerm, toeBermFields, infoP);
        BindObjectToUI(zone.Caracteristics.Foundation, foundationFields, infoF);
    }

    string[] infoGeral = new string[]
    {
        "Ano de Levantamento", "Coordenada X1", "Coordenada Y1", "Coordenada X2",
        "Coordenada Y2", "Zona", "Comprimento (m)", "Largura (m)",
        "Profundidade Máxima (m)", "Profundidade Mínima (m)"
    };

    string[] infoC = new string[]
    {
        "Tipo", "Z5 - Cota de Fundação (m)", "Z6 - Cota do passadiço (m)", "L2 - Largura do passadiço (m)",
        "Z10 - Cota do muro cortina (m)", "Deflector", "(L1+L2+L3)- Largura do Coroamento (m)", "Cota de fundação do dente (m)",
        "Cota do passeio (m)", "Tipo", "Peso", "Disposição",
        "Natureza", "Peso Específico"
    };

    string[] infoMR = new string[]
    {
        "i1 - Inclinação", "i1a - Inclinação", "Z4 - Cota Máxima (m)", "Z3 - Cota Mínima (m)", "Z12 - Cota Intermédia (m)",
        "Tipo 1", "Peso 1", "Disposição 1", "Natureza 1", "Peso Específico 1",
        "Tipo 2", "Peso 2", "Disposição 2", "Natureza 2", "Peso Específico 2"
    };

    string[] infoT = new string[]
    {
        "i2 - Inclinação", "i2a - Inclinação", "Z7 - Cota Máxima (m)", "Z8 - Cota Mínima (m)",
        "Z13 - Cota Intermédia (m)", "Tipo 1", "Peso 1", "Disposição 1",
        "Natureza 1", "Peso Específico 1", "Tipo 2", "Peso 2",
        "Disposição 2", "Natureza 2", "Peso Específico 2"
    };

    string[] infoBC = new string[]
    {
        "Z4 - Cota (m)", "L3 - Largura (m)", "Tipo", "Peso",
        "Disposição", "Natureza", "Peso Específico"
    };

    string[] infoBCI = new string[]
    {
        "Z7 - Cota (m)", "L1 - Largura (m)", "Tipo", "Peso",
        "Disposição", "Natureza", "Peso Específico"
    };

    string[] infoP = new string[]
    {
        "i3 - Inclinação", "Z2 - Cota Máxima (m)", "Z1 - Cota Mínima (m)", "L4 - Largura (m)",
        "Tipo", "Peso", "Disposição",
        "Natureza", "Peso Específico"
    };

    string[] infoF = new string[]
    {
        "Z1 - Cota superior (m)", "Z11 - Cota inferior (m)", "L5 - Largura (m)",
        "Inclinação", "Tipo", "Peso", "Disposição",
        "Natureza", "Peso Específico"
    };
}
