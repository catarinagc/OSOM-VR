using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class TasksManager : MonoBehaviour
{
    List<string> tasks = new List<string>
    {
        "Navegue até ao Hotspot 17 e, na direção T, determine se entre 2018 e 2022, houve alguma alteração na posição dos blocos",
        "Faça zoom numa imagem de 2018 no Hotspot 10",
        "Determine o grau de risco global do Troço C",
        "Identifique o grau de evolução do Manto Resistente no Troço A",
        "Identifique se houve alguma diferença no estado do talude no Manto Resistente do Troço D entre 2023 e 2018",
        "Crie uma anotação no hotspot 15, imagem de 2020, na direção F que diga \"rotação do bloco\"",
        "Tire um screenshot que contenha a informação sobre o nível de opinião geral da inspeção submarina do Troço A em 2023."
    };

    private List<int> remainingIndices = new List<int>();

    [SerializeField] TMP_Text taskDescription;
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
    }
}
