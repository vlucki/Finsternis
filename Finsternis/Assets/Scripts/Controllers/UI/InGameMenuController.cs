namespace Finsternis
{
    using System.Collections.Generic;

    #region using Unity
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
    #endregion

    using UnityQuery;
    using MovementEffects;

    [RequireComponent(typeof(CanvasGroup), typeof(InputRouter))]
    [DisallowMultipleComponent]
    public class InGameMenuController : MonoBehaviour
    {
        private static int InstanceCount = 0;

        private CanvasGroup group;
        private List<Button> options;
        private Circle menuBounds;
        private bool transitioning;
        private float targetPercentage;

        public UnityEvent OnMenuOpen;
        public UnityEvent OnMenuClose;

        private UnityEvent OnTransitionFinished;

        void Awake()
        {
            if (InstanceCount > 0)
            {
                Log.Warn("There should only be one instance of the InGame menu at any given time.");
                Destroy(this.gameObject);
            }
            else
            {
                this.group = GetComponent<CanvasGroup>();
                this.menuBounds = new Circle(GetComponent<RectTransform>().sizeDelta.Min()/2, GetComponent<RectTransform>().anchoredPosition);
                this.options = new List<Button>();
                this.OnMenuClose.AddListener(() => gameObject.SetActive(false));
                
                GetComponentsInChildren<Button>(options);
                if (options.Count <= 0)
                    Log.Warn("Not a single option found on the menu.");
            }
        }

        public void Toggle()
        {
            if (transitioning)
                return;
            if (!gameObject.activeSelf)
                Open();
            else
                Close();
        }

        /// <summary>
        /// Sets up everything in order for the menu to open (animations, effects, callbacks)
        /// </summary>
        public void Open()
        {
            this.OnMenuOpen.Invoke();
            gameObject.SetActive(true);
            this.targetPercentage = 1;
            this.OnTransitionFinished = null;
            if(!transitioning)
                Timing.RunCoroutine(_ToggleMenu());
        }
        
        /// <summary>
        /// Sets up everything in order for the menu to close (animations, effects, callbacks)
        /// </summary>
        public void Close()
        {
            this.targetPercentage = 0;
            this.OnTransitionFinished = this.OnMenuClose;
            if(!transitioning)
                Timing.RunCoroutine(_ToggleMenu());
        }

        /// <summary>
        /// Animates the menu, interpolating it's current and target state (eg. from Closed to Opened)
        /// </summary>
        private IEnumerator<float> _ToggleMenu()
        {
            transitioning = true;
            float currentPercentage = 1 - this.targetPercentage;
            do
            {
                PositionButtons(currentPercentage);
                currentPercentage = Mathf.Lerp(currentPercentage, targetPercentage, 0.2f);
                yield return 0;
            }
            while (Mathf.Abs(currentPercentage - targetPercentage) > 0.2f);

            PositionButtons(targetPercentage); //call once more to avoid precision errors

            if (options.Count > 0)
                options[0].Select();

            if (this.OnTransitionFinished != null) this.OnTransitionFinished.Invoke();

            transitioning = false;            
        }

        /// <summary>
        /// Position every button within the menu in a circular fashion.
        /// </summary>
        /// <param name="percentage">How far from the center should the buttons be? (0 = centered, 1 = the menu radius. </param>
        private void PositionButtons(float percentage = 1)
        {
            float angleBetweenOptions = -360 / options.Count;
            Vector2 optionPosition = this.menuBounds.center + Vector2.up * this.menuBounds.radius;
            for(int index = 0; index < options.Count; index++)
            {
                this.options[index].GetComponent<RectTransform>().anchoredPosition = optionPosition;
                var dir = optionPosition - menuBounds.center;
                dir = dir.Rotate(angleBetweenOptions);
                optionPosition = dir.normalized * this.menuBounds.radius * percentage;
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
                GameManager.Instance.Exit();
            }
        }
    }
}
