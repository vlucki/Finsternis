using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MovementEffects;

public class LoadingScreenController : MonoBehaviour {

    public static string sceneToLoad;
    private string loadingScreenScene;

    void Awake()
    {
        loadingScreenScene = SceneManager.GetActiveScene().name;
    }

	void Start () {
        if(!string.IsNullOrEmpty(sceneToLoad))
           Timing.RunCoroutine(_Load(SceneManager.LoadSceneAsync(sceneToLoad)));     
	}

    private IEnumerator<float> _Load(AsyncOperation loadScene)
    {
        while (!loadScene.isDone)
        {
            yield return 0f;
        }

        SceneManager.UnloadScene(loadingScreenScene);
    }
}
