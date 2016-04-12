using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroFade : MonoBehaviour {

    [Range(0, 10)]
    public float fadeOutTime = 5;
    public bool skippable = true;
    public KeyCode skipKey = KeyCode.Escape;
    public Image imageToFade;
    public string sceneToLoad;

    private bool _loadingNextScene = false;

    void Start () {
        if (imageToFade)
        {
            imageToFade.CrossFadeAlpha(0, fadeOutTime / 2, false);
        }
        StartCoroutine("countDown");
	}

    private IEnumerator countDown(){
        yield return new WaitForSeconds(5);
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (!_loadingNextScene)
            SceneManager.LoadScene(sceneToLoad);
    }

    public void Update()
    {
        if (skippable && Input.GetKeyDown(skipKey))
        {
            LoadNextScene();
        }
    }
    
}
