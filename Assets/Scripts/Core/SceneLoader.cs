using UnityEngine;
using UnityEngine.SceneManagement;

namespace BelajarAksara.Core
{
  public class SceneLoader : MonoBehaviour
  {
    public static SceneLoader Instance { get; private set; }

    private void Awake()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(gameObject);
        return;
      }

      Instance = this;
      DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
      SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(string sceneName)
    {
      StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private System.Collections.IEnumerator LoadSceneRoutine(string sceneName)
    {
      AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
      while (!operation.isDone)
      {
        yield return null;
      }
    }
  }
}
