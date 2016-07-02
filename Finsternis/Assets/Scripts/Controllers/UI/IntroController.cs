using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using MovementEffects;

public class IntroController : MonoBehaviour {

    [SerializeField]
    [Range(0, 10)]
    private float _fadeInTime = 2;

    [SerializeField]
    [Range(0, 10)]
    private float _fadeOutTime = 1;

    [SerializeField]
    [Range(0, 10)]
    private float _delayAfterFadeIn = 1;

    [SerializeField]
    [Range(0, 10)]
    private float _delayAfterFadeOut = 0.5f;

    [SerializeField]
    private bool _skippable = true;

    [SerializeField]
    private Image[] _imagesToShow;

    [SerializeField]
    private string _sceneToLoad = "MainMenu";

    private bool _loadingNextScene = false;
    private Queue<Image> _imagesQueue;
    private Image currentImage;
    private IEnumerator<float> transitionHandle;

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
        transitionHandle = Timing.RunCoroutine(_Transition(FadeOut, _fadeInTime, 1, _delayAfterFadeIn));
    }

    private void FadeOut()
    {
        _skippable = false;
        Action nextStep = LoadNextScene;
        if (_imagesQueue.Count > 0)
            nextStep = FadeIn;
        transitionHandle = Timing.RunCoroutine(_Transition(nextStep, _fadeOutTime, 0, _delayAfterFadeOut));
    }

    private IEnumerator<float> _Transition(Action callback, float fadeTime, float targetAlpha, float holdTime = 0f)
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
        yield return Timing.WaitForSeconds(fadeTime + holdTime);
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
            Timing.KillCoroutine(transitionHandle);
            FadeOut();
        }
    }
    
}
