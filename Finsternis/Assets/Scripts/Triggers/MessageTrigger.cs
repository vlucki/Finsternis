namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityQuery;

    public class MessageTrigger : Trigger
    {

        [SerializeField]
        private string messageToDisplay;

        private MessageController displayedMessage;

        protected override void Awake()
        {
            base.Awake();

            onEnter.AddListener(ShowMessage);
            onExit.AddListener(HideMessage);
        }

        public void SetMessage(string message)
        {
            this.messageToDisplay = message;
        }

        private void HideMessage(GameObject obj)
        {
            if (this.messageToDisplay.IsNullOrEmpty())
                return;

            if (this.displayedMessage && this.displayedMessage.isActiveAndEnabled)
                this.displayedMessage.Hide();
        }

        private void ShowMessage(GameObject obj)
        {
            if (this.messageToDisplay.IsNullOrEmpty())
                return;

            if (this.displayedMessage && this.displayedMessage.isActiveAndEnabled)
                this.displayedMessage.Hide();

            this.displayedMessage = MessagesManager.Instance.ShowMessage(transform.position.WithY(1), this.messageToDisplay);
        }

        void OnDisable()
        {
            if (this.displayedMessage && this.displayedMessage.isActiveAndEnabled)
                this.displayedMessage.Hide();
        }
    }
}