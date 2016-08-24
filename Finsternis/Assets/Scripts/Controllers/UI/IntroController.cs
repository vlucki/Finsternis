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
    private Graphic[] _graphicsToFade;

    [SerializeField][SceneSelection]
    private string _sceneToLoad;

    private bool _loadingNextScene = false;
    private Queue<Graphic> _graphicsQueue;
    private Graphic _currentGraphic;

    private IEnumerator<float> _transitionHandle;

    void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(_sceneToLoad, "A scene to load must be defined!");
        UnityEngine.Assertions.Assert.AreNotEqual(_sceneToLoad, "", "A scene to load must be defined!");

        if (_graphicsToFade != null && _graphicsToFade.Length > 0)
        {
            _graphicsQueue = new Queue<Graphic>(_graphicsToFade);

            foreach (Graphic g in _graphicsToFade)
            {
                g.canvasRenderer.SetAlpha(0);
            }
        }
    }

    void Start () {
        FadeIn();
	}

    private void FadeIn()
    {
        _skippable = true;
        _transitionHandle = Timing.RunCoroutine(_Transition(FadeOut, _fadeInTime, 1, _delayAfterFadeIn));
    }

    private void FadeOut()
    {
        _skippable = false;
        Action nextStep = LoadNextScene;
        if (_graphicsQueue.Count > 0)
            nextStep = FadeIn;
        _transitionHandle = Timing.RunCoroutine(_Transition(nextStep, _fadeOutTime, 0, _delayAfterFadeOut));
    }

    private IEnumerator<float> _Transition(Action callback, float fadeTime, float targetAlpha, float holdTime = 0f)
    {
        Graphic toFade = _currentGraphic;
        if (toFade)
        {
            _currentGraphic = null;
        }
        else if (_graphicsQueue != null && _graphicsQueue.Count > 0)
        {
            toFade = _graphicsQueue.Dequeue();
            toFade.enabled = true;
            _currentGraphic = toFade;
        }
        if(toFade)
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

    public void Skip()
    {
        if (_skippable)
        {
            Timing.KillCoroutines(_transitionHandle);
            FadeOut();
        }
    }
    
}
