namespace Finsternis
{
    using System.Collections.Generic;

    #region using Unity
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    #endregion

    using UnityQuery;
    using System.Collections;

    [RequireComponent(typeof(InputRouter))]
    [DisallowMultipleComponent]
    public class InGameMenuController : MenuController
    {
        private List<MenuButtonController> options;
        private Circle menuBounds;
        private MenuEyeController eyeController;
        private bool transitioning;
        private float targetPercentage;

        private MenuButtonController lastSelected;

        private UnityAction showNewGameDialog;
        private UnityAction showExitGameDialog;

        protected override void Awake()
        {
            base.Awake();

            this.menuBounds = new Circle(GetComponent<RectTransform>().sizeDelta.Min() / 2, GetComponent<RectTransform>().anchoredPosition);
            this.options = new List<MenuButtonController>();
            this.eyeController = GetComponentInChildren<MenuEyeController>();

            OnOpen.AddListener(() =>
            {
                if (!lastSelected)
                    lastSelected = options[0];

                if (!lastSelected.IsSelected)
                    lastSelected.Select();
                else
                    eyeController.LookAt(lastSelected.gameObject);

                if (EvtSystem.currentSelectedGameObject != lastSelected.gameObject)
                    EvtSystem.SetSelectedGameObject(lastSelected.gameObject);
            });

            GetComponentsInChildren<MenuButtonController>(this.options);

            if (this.options.Count <= 0)
                this.Warn("Not a single option found on the menu.");

            else if (this.options.Count > 1)
            {
                InitOptions();
            }

            showNewGameDialog = new UnityAction(() =>
                ConfirmationDialogController.Show("Start a new game?\n(current progress will be lost)", GameManager.Instance.NewGame, BeginOpening)
            );

            showExitGameDialog = new UnityAction(() =>
                ConfirmationDialogController.Show("Exit game?", GameManager.Instance.Exit, BeginOpening)
            );
        }

        private void InitOptions()
        {
            for (int i = 0; i < this.options.Count; i++)
            {
                int left = i - 1;
                int right = i + 1;
                if (left < 0)
                    left = this.options.Count - 1;
                if (right >= this.options.Count)
                    right = 0;
                Navigation n = options[i].navigation;
                n.selectOnRight = options[right];
                n.selectOnLeft = options[left];
                options[i].OnSelectionChanged.AddListener(
                    (selected, button) => lastSelected = selected ? (MenuButtonController)button : lastSelected);
            }
        }

        /// <summary>
        /// Sets up everything in order for the menu to open (animations, effects, callbacks)
        /// </summary>
        public override void BeginOpening()
        {
            this.targetPercentage = 1;
            OnFinishedToggling.AddListener(Open);
            base.BeginOpening();
            StartCoroutine(_ToggleMenu());
        }

        /// <summary>
        /// Sets up everything in order for the menu to close (animations, effects, callbacks)
        /// </summary>
        public override void BeginClosing()
        {
            this.targetPercentage = 0;
            this.eyeController.Reset();
            OnFinishedToggling.AddListener(Close);
            base.BeginClosing();
            StartCoroutine(_ToggleMenu());
        }

        /// <summary>
        /// Animates the menu, interpolating it's current and target state (eg. from Closed to Opened)
        /// </summary>
        protected IEnumerator _ToggleMenu()
        {
            if (!transitioning)
            {
                transitioning = true;
                float currentPercentage = 1 - this.targetPercentage;
                do
                {
                    PositionButtons(currentPercentage);
                    currentPercentage = Mathf.Lerp(currentPercentage, targetPercentage, 0.2f);
                    yield return null;
                }
                while (Mathf.Abs(currentPercentage - targetPercentage) > 0.2f);

                PositionButtons(targetPercentage); //call once more to avoid precision errors

                OnFinishedToggling.Invoke();
                OnFinishedToggling.RemoveAllListeners();

                transitioning = false;
            }
        }

        /// <summary>
        /// Position every button within the menu in a circular fashion.
        /// </summary>
        /// <param name="percentage">How far from the center should the buttons be? (0 = centered, 1 = the menu radius. </param>
        private void PositionButtons(float percentage = 1)
        {
            float angleBetweenOptions = -360 / options.Count;
            Vector2 optionPosition = this.menuBounds.center + Vector2.up * this.menuBounds.radius;
            for (int index = 0; index < options.Count; index++)
            {
                this.options[index].GetComponent<RectTransform>().anchoredPosition = optionPosition;
                var dir = optionPosition - menuBounds.center;
                dir = dir.Rotate(angleBetweenOptions);
                optionPosition = dir.normalized * this.menuBounds.radius * percentage;
            }
        }

        public void NewGame(bool askForConfirmation = true)
        {

            if (askForConfirmation)
            {
                BeginClosing();
                OnFinishedToggling.AddListener(showNewGameDialog);
            }
            else
            {
                GameManager.Instance.NewGame();
            }
        }

        public void Exit(bool askForConfirmation = true)
        {
            if (askForConfirmation)
            {
                BeginClosing();
                OnFinishedToggling.AddListener(showExitGameDialog);
            }
            else
            {
                GameManager.Instance.Exit();
            }
        }
    }
}
