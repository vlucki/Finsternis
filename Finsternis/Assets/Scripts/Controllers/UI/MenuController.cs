using UnityEngine;
using UnityEngine.UI;
namespace Finsternis
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(Selectable))]
    [RequireComponent(typeof(Follow))]
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private GameManager gameManager;

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private GameObject optionsContainer;

        [SerializeField]
        private Follow followBehaviour;

        private Button[] options;

        private Selectable selectable;

        private bool isToggleButtonDown;

        float targetAlpha = 0;
        [SerializeField]
        [Range(0, 1)]
        float fadeSpeed = 0.2f;

        private CanvasGroup group;

        public bool Active
        {
            get { return this.canvas.enabled; }
            set
            {
                this.canvas.enabled = value;
                this.followBehaviour.enabled = value;
                this.optionsContainer.SetActive(value);
            }
        }

        void Awake()
        {
            if (!this.gameManager)
                this.gameManager = GameManager.Instance;

            if (!this.canvas)
                this.canvas = GetComponent<Canvas>();

            if (!this.selectable)
                this.selectable = GetComponent<Selectable>();

            if (!this.group)
                this.group = GetComponent<CanvasGroup>();

            this.group.alpha = 0;

            if (!this.optionsContainer)
            {
                foreach (Transform t in transform)
                {
                    if (t.name.Equals("OptionsContainer"))
                    {
                        this.optionsContainer = t.gameObject;
                        break;
                    }
                }
            }

            if (!this.followBehaviour)
                this.followBehaviour = GetComponent<Follow>();

            if (this.options == null)
                this.options = this.optionsContainer.GetComponentsInChildren<Button>();
        }

        void Start()
        {
            Active = false;
        }

        void Update()
        {
            if (Input.GetAxis("Cancel") > 0)
            {
                if (!this.isToggleButtonDown)
                {
                    ToggleMenu();
                    this.isToggleButtonDown = true;
                }
            }
            else if (this.isToggleButtonDown)
            {
                this.isToggleButtonDown = false;
            }

            if (!Mathf.Approximately(this.group.alpha, this.targetAlpha))
                this.group.alpha = Mathf.Lerp(this.group.alpha, this.targetAlpha, fadeSpeed);

            if (Active && this.group.alpha < 0.1f)
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
            this.followBehaviour.ResetOffset();
            this.targetAlpha = 1;
            this.followBehaviour.Target.GetComponent<CharController>().Lock();

            this.selectable.Select();
            this.options[0].Select();

            this.transform.position = this.followBehaviour.Target.transform.position + 2 * this.followBehaviour.offset;
        }

        public void Hide(bool usedToggleButton = false)
        {
            this.isToggleButtonDown = usedToggleButton;
            this.targetAlpha = 0;
            this.followBehaviour.Target.GetComponent<CharController>().UnlockWithDelay(0.3f);
            this.followBehaviour.offset.x *= -1;
        }

        public void MainMenu(bool askForConfirmation = true)
        {
            if (askForConfirmation)
            {
                ConfirmationDialogController.Show("Are you sure you wish to quit the game?", MainMenu, false, this.options[2].Select);
            }
            else
            {
                this.gameManager.LoadScene("MainMenu");
            }
        }

        public void Exit(bool askForConfirmation = true)
        {
            if (askForConfirmation)
            {
                ConfirmationDialogController.Show("Are you sure you wish to quit the game?", Exit, false, this.options[3].Select);
            }
            else
            {
                this.gameManager.Exit();
            }
        }
    }
}