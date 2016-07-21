using UnityEngine;
using UnityEngine.UI;
namespace Finsternis
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(Selectable))]
    [RequireComponent(typeof(Follow))]
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private GameManager _gameManager;

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
            if (!_gameManager)
                _gameManager = GameManager.Instance;

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
                _options = _optionsContainer.GetComponentsInChildren<Button>();
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
                    ToggleMenu();
                    _isToggleButtonDown = true;
                }
            }
            else if (_isToggleButtonDown)
            {
                _isToggleButtonDown = false;
            }

            if (!Mathf.Approximately(_group.alpha, _targetAlpha))
                _group.alpha = Mathf.Lerp(_group.alpha, _targetAlpha, fadeSpeed);

            if (Active && _group.alpha < 0.1f)
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
            _followBehaviour.Target.GetComponent<CharController>().Lock();
            //GameManager.Instance.Player.GetComponent<InputRouter>().enabled = false;

            _selectable.Select();
            _options[0].Select();

            this.transform.position = this._followBehaviour.Target.transform.position + 2 * this._followBehaviour.offset;
        }

        public void Hide(bool usedToggleButton = false)
        {
            _isToggleButtonDown = usedToggleButton;
            _followBehaviour.Target.GetComponent<CharController>().UnlockWithDelay(1f);
            _targetAlpha = 0;
            _followBehaviour.offset.x *= -1;
        }

        public void MainMenu(bool askForConfirmation = true)
        {
            if (askForConfirmation)
            {
                ConfirmationDialogController.Show("Are you sure you wish to quit the game?", MainMenu, false, _options[2].Select);
            }
            else
            {
                _gameManager.LoadScene("MainMenu");
            }
        }

        public void Exit(bool askForConfirmation = true)
        {
            if (askForConfirmation)
            {
                ConfirmationDialogController.Show("Are you sure you wish to quit the game?", Exit, false, _options[3].Select);
            }
            else
            {
                _gameManager.Exit();
            }
        }
    }
}