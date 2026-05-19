using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class ZoneInspectionsUI_Manager : MonoBehaviour
{
    [SerializeField] TMP_Text title_text;
    [SerializeField] TMP_Dropdown dropdown;

    [System.Serializable] 
    public class UIField
    {
        public string fieldName;
        public TMP_Text labelText;
        public TMP_Text text;
    }

    [SerializeField] List<UIField> generalFields;
    [SerializeField] List<UIField> resistentArmorFields;
    [SerializeField] List<UIField> superstructureFields;
    [SerializeField] List<UIField> interiorArmorFields;
    [SerializeField] List<UIField> underwaterields;
    [SerializeField] List<UIField> pierFields;

    private int yearSelected;
    private Zone currentZone;
    private string default_title_text;

    public void ChangeYearSelected(int index)
    {
        yearSelected = currentZone.Inspections[index].Year;
        OpenInspectionData();
    }
    
    // void BindObjectToUI(object data, List<UIField> fields)
    // {
    //     var type = data.GetType();

    //     foreach (var fieldUI in fields)
    //     {
    //         var field = type.GetField(fieldUI.fieldName);

    //         if (field == null)
    //         {
    //             fieldUI.text.text = "-";
    //             continue;
    //         }

    //         var value = field.GetValue(data);
    //         fieldUI.text.text = value != null ? value.ToString() : "-";
    //     }
    // }
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

            fieldUI.labelText.text = label;
            fieldUI.text.text = valueStr;
        }
    }
    
    void Awake()
    {
        default_title_text = "Portimão Poente ";
    }

    public void PrepareOpen(Zone zone, int year)
    {
        title_text.text = default_title_text + "(" + zone.name + ")";
        currentZone = zone;
        yearSelected = year;
        PrepareDropdown();
        OpenInspectionData();
    }

    private void PrepareDropdown()
    {
        dropdown.ClearOptions();

        List<string> options = new List<string>();
        int selectedIndex = 0;

        for (int i = 0; i < currentZone.Inspections.Count; i++)
        {
            int year = currentZone.Inspections[i].Year;
            options.Add(year.ToString());

            if (year == yearSelected)
            {
                selectedIndex = i;
            }
        }

        dropdown.AddOptions(options);

        dropdown.SetValueWithoutNotify(selectedIndex);

        dropdown.RefreshShownValue();

    }

    private void OpenInspectionData()
    {
        Debug.Log(yearSelected);
        Inspection inspection = currentZone.GetInspectionFromYear(yearSelected);

        inspection.ResistentArmorLayer.PrepareTexts();
        inspection.Superstructure.PrepareTexts();
        inspection.InteriorArmorLayer.PrepareTexts();
        inspection.Underwater.PrepareTexts();
        inspection.Pier.PrepareTexts();

        BindObjectToUI(inspection.General, generalFields, inspGeral);
        BindObjectToUI(inspection.ResistentArmorLayer, resistentArmorFields, inspResArmor);
        BindObjectToUI(inspection.Superstructure, superstructureFields, inspSup);
        BindObjectToUI(inspection.InteriorArmorLayer, interiorArmorFields, inspIntArmor);
        BindObjectToUI(inspection.Underwater, underwaterields, inspSub);
        BindObjectToUI(inspection.Pier,pierFields, inspPier);
    }

    string[] inspGeral = new string[]
    {
        "Relevante", "Motivo da Relevância", "Notas Gerais"
    };

    string[] inspResArmor = new string[]
    {
        "Quedas", "Fraturas", "Talude", 
        "Quantidade", "Descrição", "Som",
        "Junto à linha de água (m)", "Coroamento (m)", "Maior assentamento", 
        "Obeservações", "Opinião Geral", "Estado"
    };

    string[] inspSup = new string[]
    {
        "Fraturas",
        "Quantidade", "Descrição", "Assentamento", "Derrubamento", "Deslizamento",
        "Obeservações", "Opinião Geral", "Estado"
    };

    string[] inspIntArmor = new string[]
    {
        "Quedas", "Fraturas", "Talude",
        "Quantidade", "Descrição", "Som",
        "Junto à linha de água (m)", "Coroamento (m)", "Maior assentamento", 
        "Obeservações", "Opinião Geral", "Estado"
    };

    string[] inspSub = new string[]
    {
        "Quedas", "Fraturas", "Talude", "Quantidade",
        "Quedas", "Fraturas", "Talude", "Quantidade",
        "Quedas", "Fraturas", "Talude", "Quantidade",
        "Obeservações", "Opinião Geral"
    };

    string[] inspPier = new string[]
    {
        "Fraturas", "Betão", "Assentamento (m)", "Deslizamento (m)", "Rotação (º)",
        "Fraturas", "Betão", "Obstrução de orifícios", "Deslocamento das juntas",
        "Obeservações", "Opinião Geral"
    };
}
