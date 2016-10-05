namespace Finsternis
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MenuController : ExtendedBehaviour
    {
        private EventSystem evtSystem;
        protected EventSystem EvtSystem
        {
            get
            {
                if(!this.evtSystem)
                    this.evtSystem = FindObjectOfType<EventSystem>();
                return this.evtSystem;
            }
        }
        [Header("Transition events")]
        [Space]
        public UnityEvent OnBeganOpening;
        public UnityEvent OnBeganClosing;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;

        private UnityEvent onFinishedToggling;

        public bool SkipCloseEvent { get; set; }

        protected UnityEvent OnFinishedToggling
        {
            get
            {
                if (onFinishedToggling == null)
                    onFinishedToggling = new UnityEvent();
                return onFinishedToggling;
            }
        }

        private CanvasGroup canvasGroup;

        protected CanvasGroup CanvasGroup {
            get {
                if(!this.canvasGroup)
                    this.canvasGroup = GetComponent<CanvasGroup>();
                return this.canvasGroup;
            }
        }
        
        public bool IsOpen { get; private set; }

        public void Toggle(bool immediately = false)
        {
            if (!IsOpen)
            {
                if (immediately)
                    Open();
                else
                    BeginOpening();
            }
            else
            {
                if (immediately)
                    Close();
                else
                    BeginClosing();
            }
        }

        /// <summary>
        /// Opens the menu
        /// </summary>
        public virtual void BeginOpening()
        {
            gameObject.SetActive(true);
            this.OnBeganOpening.Invoke();
        }

        /// <summary>
        /// Closes the menu
        /// </summary>
        public virtual void BeginClosing()
        {
            this.OnBeganClosing.Invoke();
            this.CanvasGroup.interactable = false;
        }

        public virtual void Open()
        {
            IsOpen = true;
            this.CanvasGroup.interactable = true;
            SkipCloseEvent = false;
            OnOpen.Invoke();
        }

        public virtual void Close()
        {
            IsOpen = false;
            if(!SkipCloseEvent)
                OnClose.Invoke();
            gameObject.SetActive(false);
        }
    }
}
