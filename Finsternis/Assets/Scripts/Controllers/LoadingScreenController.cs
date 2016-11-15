using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenController : MonoBehaviour {

    public static string sceneToLoad;
    private string loadingScreenScene;

    void Awake()
    {
        loadingScreenScene = SceneManager.GetActiveScene().name;
    }

	void Start () {
        if(!string.IsNullOrEmpty(sceneToLoad))
           StartCoroutine(_Load(SceneManager.LoadSceneAsync(sceneToLoad)));     
	}

    private IEnumerator _Load(AsyncOperation loadScene)
    {
        while (!loadScene.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadScene(loadingScreenScene);
    }
}
