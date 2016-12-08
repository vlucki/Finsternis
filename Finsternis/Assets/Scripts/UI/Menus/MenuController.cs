namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(CanvasGroup), typeof(Selectable))]
    [DisallowMultipleComponent]
    public abstract class MenuController : CustomBehaviour
    {
        [Serializable]
        public class MenuEvent : CustomEvent<MenuController> { }

        [Serializable]
        public struct TransitionEvents
        {
            public UnityEvent onBeganOpening;
            public UnityEvent onBeganClosing;
            public MenuEvent  onOpen;
            public MenuEvent  onClose;
        }

        #region Variables
        private EventSystem evtSystem;
        protected UnityEvent onFinishedToggling;

        [SerializeField]
        private bool keepPlayerLocked = true;

        [SerializeField]
        private TransitionEvents events;
        #endregion

        #region Properties

        public UnityEvent OnBeganOpening { get { return this.events.onBeganOpening; } }
        public MenuEvent OnOpen { get { return this.events.onOpen; } }
        public UnityEvent OnBeganClosing { get { return this.events.onBeganClosing; } }
        public MenuEvent OnClose { get { return this.events.onClose; } }

        public bool SkipCloseEvent { get; set; }

        protected EventSystem EventSystem
        {
            get
            {
                if (!this.evtSystem)
                    this.evtSystem = FindObjectOfType<EventSystem>();
                return this.evtSystem;
            }
        }

        public CanvasGroup CanvasGroup { get { return GetCachedComponent<CanvasGroup>(true); } }

        public Selectable Selectable { get { return GetCachedComponent<Selectable>(true); } }

        public bool IsOpening { get; private set; }
        public bool IsOpen { get; private set; }
        #endregion

        #region methods

        protected override void Awake()
        {
            base.Awake();
            var nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            Selectable.navigation = nav;

            onFinishedToggling = new UnityEvent();
            if (this.keepPlayerLocked)
            {
                if (!GameManager.Instance.Player)
                    GameManager.Instance.onPlayerSpawned.AddListener(AddPlayerCheck);
                else
                    AddPlayerCheck(GameManager.Instance.Player);
            }
        }

        private void AddPlayerCheck(CharController player)
        {
            GameManager.Instance.onPlayerSpawned.RemoveListener(AddPlayerCheck);
            player.onUnlock.AddListener(LockPlayerBack);
        }

        private void LockPlayerBack(CharController player)
        {
            if (this.IsOpen || this.IsOpening)
                player.Lock();
        }

        void OnDestroy()
        {
            GameManager.Instance.onPlayerSpawned.RemoveListener(AddPlayerCheck);
        }

        /// <summary>
        /// Opens the menu if it's closed or close it if it's open.
        /// </summary>
        /// <param name="immediately">Wheter BeginOpening/Closing should be skipped or not.</param>
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
        /// Sets the menu up for being opened, activating its game object.
        /// </summary>
        public virtual void BeginOpening()
        {
            this.gameObject.SetActive(true);

            if (this.IsOpen)
                return;

            if (this.keepPlayerLocked && GameManager.Instance.Player)
                GameManager.Instance.Player.Lock();
            IsOpening = true;
            this.OnBeganOpening.Invoke();
        }

        /// <summary>
        /// Sets the menu up for being closed.
        /// </summary>
        public virtual void BeginClosing()
        {
            this.OnBeganClosing.Invoke();
            this.CanvasGroup.interactable = false;
            this.Selectable.interactable = false;
        }

        /// <summary>
        /// Opens the menus
        /// </summary>
        public virtual void Open()
        {
            if (this.IsOpen)
                return;

            IsOpening = false;
            IsOpen = true;
            this.CanvasGroup.interactable = true;
            this.Selectable.interactable = true;
            this.Selectable.Select();
            SkipCloseEvent = false;
            OnOpen.Invoke(this);
        }

        /// <summary>
        /// Closes the menu and deactivate its gameobject.
        /// </summary>
        public virtual void Close()
        {
            IsOpen = false;
            if (!SkipCloseEvent)
                OnClose.Invoke(this);
            gameObject.SetActive(false);

            if (this.keepPlayerLocked && GameManager.Instance.Player)
                GameManager.Instance.Player.Unlock();
        }
    }
    #endregion
}
