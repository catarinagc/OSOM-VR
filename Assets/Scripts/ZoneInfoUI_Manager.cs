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

            fieldUI.text.text = $"{label}: {valueStr}";
        }
    }

    private string default_title_text;
    
    void Awake()
    {
        default_title_text = "Portimão Poente ";
    }

    public void PrepareOpen(Zone zone)
    {
        title_text.text = default_title_text + "(" + zone.name + ")";

        BindObjectToUI(zone.Caracteristics.General, generalFields, infoGeral);
        BindObjectToUI(zone.Caracteristics.Superstructure, superstructureFields, infoC);
        BindObjectToUI(zone.Caracteristics.InnerCrestBerm, innerCrestFields, infoMR);
        BindObjectToUI(zone.Caracteristics.InteriorArmorLayer, interiorArmorFields, infoT);
        BindObjectToUI(zone.Caracteristics.OuterCrestBerm, outerCrestFields, infoBC);
        BindObjectToUI(zone.Caracteristics.ResistentArmorLayer, resistentArmorFields, infoBCI);
        BindObjectToUI(zone.Caracteristics.ToeBerm, toeBermFields, infoP);
        BindObjectToUI(zone.Caracteristics.Foundation, foundationFields, infoF);
    }

    string[] infoGeral = new string[]
    {
        "Levantamento", "CoordenadaM1", "CoordenadaP1", "CoordenadaM2",
        "CoordenadaP2", "Zona", "Comprimento", "Largura",
        "ProfundidadeMaxima", "ProfundidadeMinima"
    };

    string[] infoC = new string[]
    {
        "Tipo", "CotadeFundacao", "CotadePIMC", "LarguradeCoroamento",
        "CotadeCoroamento", "Deflector", "LarguraPass", "CotaDente",
        "CotaPass", "TipoC", "PesoC", "DisposC",
        "NaturezaC", "PesoEspC"
    };

    string[] infoMR = new string[]
    {
        "Inclinacao", "InclinacaoSub", "CotaSuperior", "CotaInferior",
        "CotaIntM", "TipoM", "PesoM", "DisposM",
        "NaturezaM", "PesoEspM", "TipoMSub", "PesoMSub",
        "DisposMSub", "NaturezaMSub", "PesoEspMSub"
    };

    string[] infoT = new string[]
    {
        "InclinacaoT", "InclinacaoTSub", "CotaSuperiorTardoz", "CotaInferiorTardoz",
        "CotaIntT", "TipoT", "PesoT", "DisposT",
        "NaturezaT", "PesoEspT", "TipoTSub", "PesoTSub",
        "DisposTSub", "NaturezaTSub", "PesoEspTSub"
    };

    string[] infoBC = new string[]
    {
        "CotaBC", "LarguraBC", "TipoBermaExt", "PesoBermaExt",
        "DisposBermaExt", "NaturezaBermaExt", "PesoEspBermaExt"
    };

    string[] infoBCI = new string[]
    {
        "CotaBCI", "LarguraBCI", "TipoBermaInt", "PesoBermaInt",
        "DisposBermaInt", "NaturezaBermaInt", "PesoEspBermaInt"
    };

    string[] infoP = new string[]
    {
        "InclinacaoP", "CotaSuperiorPe", "CotaInferiorPe", "LarguraP",
        "TipoBanqPeTalude", "PesoBanqPeTalude", "DisposBanqPeTalude",
        "NaturezaBanqPeTalude", "PesoEspBanqPeTalude"
    };

    string[] infoF = new string[]
    {
        "FundacaoCota", "FundacaoCotaInf", "FundacaoLargura",
        "InclinacaoF", "TipoF", "PesoF", "DisposF",
        "NaturezaF", "PesoEspF"
    };
}
