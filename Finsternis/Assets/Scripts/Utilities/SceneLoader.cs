using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public void Load(string sceneName)
    {
        LoadingScreenController.sceneToLoad = sceneName;
        SceneManager.LoadScene("LoadingScreen");
    }
}
