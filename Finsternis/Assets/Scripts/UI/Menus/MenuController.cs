namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MenuController : MonoBehaviour
    {
        #region variables
        private EventSystem evtSystem;
        private UnityEvent onFinishedToggling;
        private CanvasGroup canvasGroup;

        [SerializeField]
        private bool keepPlayerLocked = true;

        [Header("Transition events")]
        [Space]
        public UnityEvent OnBeganOpening;
        public UnityEvent OnBeganClosing;

        public UnityEvent OnOpen;
        public UnityEvent OnClose;
        #endregion

        #region Properties
        protected EventSystem EvtSystem
        {
            get
            {
                if (!this.evtSystem)
                    this.evtSystem = FindObjectOfType<EventSystem>();
                return this.evtSystem;
            }
        }

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

        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (!this.canvasGroup)
                    this.canvasGroup = GetComponent<CanvasGroup>();
                return this.canvasGroup;
            }
        }

        public bool IsOpen { get; private set; }
        #endregion

        #region methods

        protected virtual void Awake()
        {
            if (this.keepPlayerLocked)
            {
                if (!GameManager.Instance.Player)
                    GameManager.Instance.OnPlayerSpawned.AddListener(AddPlayerCheck);
                else
                    AddPlayerCheck();
            }
        }

        private void AddPlayerCheck()
        {
            GameManager.Instance.Player.onUnlock.AddListener(LockPlayerBack);
        }

        private void LockPlayerBack()
        {
            if (this.IsOpen && !GameManager.Instance.Player.IsLocked)
                GameManager.Instance.Player.LockAndDisable();
        }

        void OnDestroy()
        {
            GameManager.Instance.OnPlayerSpawned.RemoveListener(AddPlayerCheck);
        }

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
            if (!SkipCloseEvent)
                OnClose.Invoke();
            gameObject.SetActive(false);
        }
    }
    #endregion
}
