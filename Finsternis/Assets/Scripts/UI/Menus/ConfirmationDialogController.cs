namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using UnityQuery;

    public class ConfirmationDialogController : MenuController
    {
        public UnityEvent onConfirm;
        public UnityEvent onCancel;

        private UnityEvent callOnClose;

        private Text yesButtonLbl;
        private Text noButtonLbl;

        [SerializeField]
        private Text messageField;

        public string Message
        {
            get { return this.messageField.text; }
            set { this.messageField.text = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            this.messageField = GetComponentInChildren<Text>();
            this.yesButtonLbl = transform.Find("YesBtn").GetComponentInChildren<Text>();
            this.noButtonLbl = transform.Find("NoBtn").GetComponentInChildren<Text>();
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

        internal void Show(string message, UnityAction confirmationCallback, UnityAction cancelationCallback = null, string confirmation = "Yes", string cancelation = "No")
        {
            Show(message, confirmation, cancelation);
            if (cancelationCallback != null)
                this.onCancel.AddListener(cancelationCallback);
            this.onConfirm.AddListener(confirmationCallback);
        }

        internal void Show<T>(string message, UnityAction<T> confirmationCallback, T confirmationParameter, UnityAction cancelationCallback = null, string confirmation = "Yes", string cancelation = "No")
        {
            Show(message, confirmation, cancelation);
            if (cancelationCallback != null)
                this.onCancel.AddListener(cancelationCallback);
            this.onConfirm.AddListener(() => confirmationCallback(confirmationParameter));
        }

        private void Show(string message, string confirmation, string cancelation)
        {
            this.BeginOpening();
            this.yesButtonLbl.text = confirmation;
            this.noButtonLbl.text = cancelation;
            this.Message = message;
        }

        void OnDisable()
        {
            onConfirm.RemoveAllListeners();
            onCancel.RemoveAllListeners();
        }
    }
}