using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class TasksManager : MonoBehaviour
{
    List<string> tasks = new List<string>
    {
        "Navegue até ao Hotspot 17 e, na direção T, compare as imagens de 2018 e 2022 e determine se houve alguma alteração na posição dos blocos",
        "Faça zoom na imagem de 2022 no Hotspot 24, direção T, e determine quantas fissuras verticais consegue identificar na base da estrutura do farolim",
        "Indique o Estado de Risco do manto resistente do troço C",
        "Compare os dados de inspeção de 2018 e 2023 quanto à degradação superficial dos materiais na Superestrutura do Troço D",
        "Crie uma anotação no hotspot 15, imagem de 2020, na direção F que diga \"bloco rodou\"",
        "Tire um screenshot contendo a informação geral das características do Troço",
    };

    private List<int> remainingIndices = new List<int>();

    [SerializeField] TMP_Text taskDescription;
    [SerializeField] TMP_Text taskTitle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetPool();
        ShowNextTask();
    }

    void ResetPool()
    {
        remainingIndices.Clear();
        for (int i = 0; i < tasks.Count; i++)
            remainingIndices.Add(i);

        for (int i = remainingIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (remainingIndices[i], remainingIndices[j]) = (remainingIndices[j], remainingIndices[i]);
        }
    }

    public void ShowNextTask()
    {
        if (remainingIndices.Count == 0)
        {
            TelemetryLogger.Instance.LogUIInteraction("Tasks over");
            taskDescription.text = "Fim das tarefas";
            return;
        }

        TelemetryLogger.Instance.LogUIInteraction("Change Task");

        int index = remainingIndices[^1];
        remainingIndices.RemoveAt(remainingIndices.Count - 1);

        string chosenTask = tasks[index];
        taskDescription.text = chosenTask;
        taskTitle.text = "Tarefa " + index.ToString();
    }
}
