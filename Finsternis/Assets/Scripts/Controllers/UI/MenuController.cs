namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using MovementEffects;

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MenuController : MonoBehaviour
    {
        public UnityEvent OnBeganOpening;
        public UnityEvent OnBeganClosing;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        private UnityEvent onFinishedToggling;

        protected UnityEvent OnFinishedToggling
        {
            get
            {
                if (onFinishedToggling == null)
                    onFinishedToggling = new UnityEvent();
                return onFinishedToggling;
            }
        }

        protected IEnumerator<float> toggleEnumerator;

        private CanvasGroup canvasGroup;

        protected CanvasGroup CanvasGroup {
            get {
                if(!this.canvasGroup)
                    this.canvasGroup = GetComponent<CanvasGroup>();
                return this.canvasGroup;
            }
        }
        
        public bool IsOpen { get; private set; }

        public virtual void Toggle()
        {
            if (!IsOpen)
                BeginOpening();
            else
                BeginClosing();
        }

        /// <summary>
        /// Opens the menu
        /// </summary>
        public virtual void BeginOpening()
        {
            this.OnBeganOpening.Invoke();

            gameObject.SetActive(true);

            if (toggleEnumerator != null)
                Timing.KillCoroutines(toggleEnumerator);

            Timing.RunCoroutine(_ToggleMenu());
        }

        /// <summary>
        /// Closes the menu
        /// </summary>
        public virtual void BeginClosing()
        {
            this.OnBeganClosing.Invoke();
            this.CanvasGroup.interactable = false;

            if (toggleEnumerator != null)
                Timing.KillCoroutines(toggleEnumerator);

            Timing.RunCoroutine(_ToggleMenu());
        }

        protected virtual void Open()
        {
            this.CanvasGroup.interactable = true;
            IsOpen = true;
            OnOpen.Invoke();
        }

        protected virtual void Close()
        {
            gameObject.SetActive(false);
            IsOpen = false;
            OnClose.Invoke();
        }

        protected abstract IEnumerator<float> _ToggleMenu();
    }
}
