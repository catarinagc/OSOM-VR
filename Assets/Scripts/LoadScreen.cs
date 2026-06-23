using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
public class LoadScreen : MonoBehaviour
{
    [SerializeField] Image progressBar;
    void Start()
    {
        //StartCoroutine(LoadGame());
    }

    void Update() { }

    private IEnumerator LoadGame()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SampleScene");
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
            yield return null;

        // Scene is fully loaded in memory, now activate it (Awake/Start will fire)
        asyncOperation.allowSceneActivation = true;

        yield return new WaitUntil(() => BreakwaterManager.IsReady && HotspotManager.IsReady && XRModeSwitcher.IsReady);

        yield return new WaitForSeconds(10f);
    }
}