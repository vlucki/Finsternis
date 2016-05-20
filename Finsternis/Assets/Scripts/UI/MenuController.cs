using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(Selectable))]
[RequireComponent(typeof(Follow))]
public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameController _gameController;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private GameObject _optionsContainer;

    [SerializeField]
    private Follow _followBehaviour;

    private Button[] _options;

    private Selectable _selectable;

    private bool _isToggleButtonDown;

    float _targetAlpha = 0;
    [SerializeField]
    [Range(0, 1)]
    float fadeSpeed = 0.2f;
    private CanvasGroup _group;

    public bool Active
    {
        get { return _canvas.enabled; }
        set
        {
            _canvas.enabled = value;
            _followBehaviour.enabled = value;
            _optionsContainer.SetActive(value);
        }
    }

    void Awake()
    {
        if (!_gameController)
            _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        if (!_canvas)
            _canvas = GetComponent<Canvas>();

        if (!_selectable)
            _selectable = GetComponent<Selectable>();

        if (!_group)
            _group = GetComponent<CanvasGroup>();

        _group.alpha = 0;

        if (!_optionsContainer)
        {
            foreach (Transform t in transform)
            {
                if (t.name.Equals("OptionsContainer"))
                {
                    _optionsContainer = t.gameObject;
                    break;
                }
            }
        }

        if (!_followBehaviour)
            _followBehaviour = GetComponent<Follow>();

        if (_options == null)
            _options = GetComponentsInChildren<Button>();
    }

    void Start()
    {
        ToggleMenu();
    }

    void Update()
    {
        if (Input.GetAxis("Cancel") > 0)
        {
            if (!_isToggleButtonDown)
            {
                if (!Active)
                    Show();
                _isToggleButtonDown = true;
            }
        }
        else if (_isToggleButtonDown)
        {
            _isToggleButtonDown = false;
        }

        if(!Mathf.Approximately(_group.alpha, _targetAlpha))
            _group.alpha = Mathf.Lerp(_group.alpha, _targetAlpha, fadeSpeed);

        if(Active && _group.alpha < 0.1f)
            Active = false;
    }

    public void ToggleMenu()
    {
        if (!Active)
            Show();
        else
            Hide();
    }

    public void Show()
    {
        Active = true;
        _followBehaviour.ResetOffset();
        _targetAlpha = 1;
        _followBehaviour.target.GetComponent<CharacterController>().Lock();

        _selectable.Select();
        _options[0].Select();

        this.transform.position = this._followBehaviour.target.transform.position + 2 * this._followBehaviour.offset;
    }

    public void Hide(bool usedToggleButton = false)
    {
        _isToggleButtonDown = usedToggleButton;
        _followBehaviour.target.GetComponent<CharacterController>().Unlock(8);
        _targetAlpha = 0;
        _followBehaviour.offset.x *= -1;
    }

    public void MainMenu(bool askForConfirmation = true)
    {
        if (askForConfirmation)
        {
            ConfirmationDialog.Show<bool>("Are you sure you wish to quit the game?", MainMenu, false, _options[2].Select);
        }
        else
        {
            _gameController.GoTo("main_menu");
        }
    }

    public void Exit(bool askForConfirmation = true)
    {
        if (askForConfirmation)
        {
            ConfirmationDialog.Show<bool>("Are you sure you wish to quit the game?", Exit, false, _options[3].Select);
        }
        else
        {
            _gameController.Exit();
        }
    }
}
