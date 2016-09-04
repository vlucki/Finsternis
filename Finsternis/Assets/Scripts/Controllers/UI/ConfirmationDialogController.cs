namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System;
    using System.Collections.Generic;

    public class ConfirmationDialogController : MenuController
    {
        public UnityEvent onConfirm;
        public UnityEvent onCancel;

        [SerializeField]
        private Text messageField;

        [SerializeField]
        private EventSystem evtSystem;

        [SerializeField]
        private GameObject yesButton;

        [SerializeField]
        private GameObject noButton;

        public static ConfirmationDialogController Instance { get; private set; }

        public string Message
        {
            get { return this.messageField.text; }
            set {  this.messageField.text = value; }
        }

        void Awake()
        {
            Instance = this;
            Init();
            Close();
        }

        void Init()
        {
            this.evtSystem = FindObjectOfType<EventSystem>();
            this.messageField = GetComponentInChildren<Text>();
            foreach (Button b in transform.GetComponentsInChildren<Button>())
            {
                if (b.name.Equals("Yes"))
                    this.yesButton = b.gameObject;
                else if (b.name.Equals("No"))
                    this.noButton = b.gameObject;
            }
        }

        void OnEnable()
        {
            if (this.evtSystem == null)
                Init();
            if (this.evtSystem.currentSelectedGameObject != this.yesButton && this.evtSystem.currentSelectedGameObject != this.noButton)
                this.evtSystem.SetSelectedGameObject(this.yesButton);
        }

        public void Confirm()
        {
            onConfirm.Invoke();
        }

        public void Cancel()
        {
            onCancel.Invoke();
        }

        internal static void Show(string message, UnityAction confirmationCallback, UnityAction cancelationCallback = null)
        {
            if (!ValidateInstance(cancelationCallback))
                return;

            Show(message);
            Instance.onConfirm.AddListener(() => confirmationCallback());
        }

        internal static void Show<T>(string message, UnityAction<T> confirmationCallback, T confirmationParameter, UnityAction cancelationCallback = null)
        {
            if (!ValidateInstance(cancelationCallback))
                return;

            Show(message);
            Instance.onConfirm.AddListener(() => confirmationCallback(confirmationParameter));
        }

        internal static void Show<T, K>(string message, UnityAction<T, K> callback, T parameterA, K parameterB, UnityAction cancelationCallback = null)
        {
            if (!ValidateInstance(cancelationCallback))
                return;

            Show(message);
            Instance.onConfirm.AddListener(() => callback(parameterA, parameterB));
        }

        private static bool ValidateInstance(UnityAction cancelationCallback)
        {
            if (Instance)
            {
                if (cancelationCallback != null)
                    Instance.onCancel.AddListener(cancelationCallback);
            }
            else if (cancelationCallback != null)
                cancelationCallback();
            
            return Instance;
        }

        private static void Show(string message)
        {
            Instance.BeginOpening();
            Instance.Message = message;
        }

        void OnDisable()
        {
            onConfirm.RemoveAllListeners();
            onCancel.RemoveAllListeners();
        }

        protected override IEnumerator<float> _ToggleMenu()
        {
            if (!IsOpen)
                Open();
            else
                Close();

            yield return 0;
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}