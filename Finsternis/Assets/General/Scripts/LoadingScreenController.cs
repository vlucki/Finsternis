using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour {
    
	void Start () {
        string toLoad = PlayerPrefs.GetString("SceneToLoad");
        StartCoroutine(Load(toLoad));        
	}

    private IEnumerator Load(string toLoad)
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(toLoad);

        while (!loadScene.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        SceneManager.UnloadScene("loading_screen");
    }
}
