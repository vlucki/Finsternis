namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class ConfirmationDialogController : MenuController
    {
        public UnityEvent onConfirm;
        public UnityEvent onCancel;

        private UnityEvent callOnClose;

        [SerializeField]
        private Text messageField;

        public static ConfirmationDialogController Instance { get; private set; }

        public string Message
        {
            get { return this.messageField.text; }
            set { this.messageField.text = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (!Instance)
            {
                Init();
            }
            Close();
        }

        void Init()
        {
            Instance = this;
            this.messageField = GetComponentInChildren<Text>();
        }

        void OnEnable()
        {
            if (!Instance)
                Init();
        }

        public override void Close()
        {
            if(callOnClose != null)
                callOnClose.Invoke();

            base.Close();
        }

        public void Confirm()
        {
            callOnClose = onConfirm;
            BeginClosing();
        }

        public void Cancel()
        {
            callOnClose = onCancel;
            BeginClosing();
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

        void OnDestroy()
        {
            if (Instance && Instance.gameObject == gameObject)
                Instance = null;
        }
    }
}