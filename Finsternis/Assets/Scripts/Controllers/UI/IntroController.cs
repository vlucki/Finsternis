using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class IntroController : MonoBehaviour {

    [Range(0, 10)]
    public float fadeInTime = 2;
    [Range(0, 10)]
    public float fadeOutTime = 1;
    [Range(0, 10)]
    public float holdTime = 1;
    public bool skippable = true;
    public Image imageToFade;
    public string sceneToLoad = "MainMenu";

    private bool _loadingNextScene = false;

    void Start () {
        StartCoroutine(Transition(FadeOut, fadeInTime, 0, true));
	}

    private void FadeOut()
    {
        skippable = false;
        StartCoroutine(Transition(LoadNextScene, fadeOutTime, 1));
    }

    private IEnumerator Transition(Action callback, float fadeTime, float targetAlpha, bool shouldHold = false)
    {
        if (imageToFade)
        {
            imageToFade.CrossFadeAlpha(targetAlpha, fadeTime, false);
        }
        yield return new WaitForSeconds(fadeTime + (shouldHold? holdTime : 0));
        callback();
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
            StopAllCoroutines();
            FadeOut();
        }
    }
    
}
