using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class IdEntryScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInputField;

    private void OnSubmitClicked()
    {
        string playerId = idInputField.text.Trim();
        if (string.IsNullOrEmpty(playerId)) return;

        TelemetryLogger.Instance.StartSession(playerId);

        // Run on the TelemetryLogger's own GameObject so the coroutine survives the scene unload
        TelemetryLogger.Instance.StartCoroutine(LoadSceneTimed("SampleScene"));
    }

    private IEnumerator LoadSceneTimed(string sceneName)
    {
        TelemetryLogger.Instance.BeginInteraction("LoadScene");

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        yield return op;

        TelemetryLogger.Instance.EndInteraction("LoadScene");
        Debug.Log("done");
        yield return new WaitForSeconds(5f);

        TelemetryLogger.Instance.BeginInteraction("TestDuration");
    }
}