using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroController : MonoBehaviour {

    [Range(0, 10)]
    public float fadeOutTime = 3;
    public bool skippable = true;
    public Image imageToFade;
    public string sceneToLoad = "MainMenu";

    private bool _loadingNextScene = false;

    void Start () {
        if (imageToFade)
        {
            imageToFade.CrossFadeAlpha(0, fadeOutTime / 2, false);
        }
        StartCoroutine(CountDown());
	}

    private IEnumerator CountDown(){
        yield return new WaitForSeconds(fadeOutTime);
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (!_loadingNextScene)
        {
            LoadingScreenController.sceneToLoad = sceneToLoad;
            SceneManager.LoadScene("LoadingScreen");
        }
    }

    public void Update()
    {
        if (skippable && Input.GetAxis("Cancel") != 0)
        {
            LoadNextScene();
        }
    }
    
}
