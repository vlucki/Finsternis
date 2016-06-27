using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour {

    public static string sceneToLoad;
    private string loadingScreenScene;

    void Awake()
    {
        loadingScreenScene = SceneManager.GetActiveScene().name;
    }

	void Start () {
        if(!string.IsNullOrEmpty(sceneToLoad))
           StartCoroutine(Load(SceneManager.LoadSceneAsync(sceneToLoad)));     
	}

    private IEnumerator Load(AsyncOperation loadScene)
    {
        while (!loadScene.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.UnloadScene(loadingScreenScene);
    }
}
