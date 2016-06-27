using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class IntroController : MonoBehaviour {

    [SerializeField]
    [Range(0, 10)]
    private float _fadeInTime = 2;

    [SerializeField]
    [Range(0, 10)]
    private float _fadeOutTime = 1;

    [SerializeField]
    [Range(0, 10)]
    private float _holdTime = 1;

    [SerializeField]
    private bool _skippable = true;

    [SerializeField]
    private Image[] _imagesToShow;

    [SerializeField]
    private string _sceneToLoad = "MainMenu";

    private bool _loadingNextScene = false;
    private Queue<Image> _imagesQueue;
    private Image currentImage;

    void Awake()
    {
        _imagesQueue = new Queue<Image>(_imagesToShow);
    }

    void Start () {
        FadeIn();
	}

    private void FadeIn()
    {
        _skippable = true;
        StartCoroutine(Transition(FadeOut, _fadeInTime, 1, true));
    }

    private void FadeOut()
    {
        _skippable = false;
        Action nextStep = LoadNextScene;
        if (_imagesQueue.Count > 0)
            nextStep = FadeIn;
        StartCoroutine(Transition(nextStep, _fadeOutTime, 0));
    }

    private IEnumerator Transition(Action callback, float fadeTime, float targetAlpha, bool shouldHold = false)
    {
        Image toFade = currentImage;
        if (toFade)
        {
            currentImage = null;
        }
        else if (_imagesQueue != null && _imagesQueue.Count > 0)
        {
            toFade = _imagesQueue.Dequeue();
            toFade.canvasRenderer.SetAlpha(0);
            toFade.enabled = true;
            currentImage = toFade;
        }
        toFade.CrossFadeAlpha(targetAlpha, fadeTime, false);
        yield return new WaitForSeconds(fadeTime + (shouldHold ? _holdTime : 0));
        callback();
    }

    private void LoadNextScene()
    {
        if (!_loadingNextScene)
        {
            LoadingScreenController.sceneToLoad = _sceneToLoad;
            SceneManager.LoadScene("LoadingScreen");
        }
    }

    public void Update()
    {
        if (_skippable && Input.GetAxis("Cancel") != 0)
        {
            StopAllCoroutines();
            FadeOut();
        }
    }
    
}
