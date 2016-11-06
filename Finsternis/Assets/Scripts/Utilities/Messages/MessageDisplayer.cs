namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityQuery;

    public class MessageDisplayer : MonoBehaviour
    {

        [SerializeField]
        protected string messageText;

        [SerializeField]
        protected Sprite messageGraphic;

        [SerializeField]
        protected Vector3 messageOffset = Vector3.up;

        [SerializeField][Tooltip("How long should this message be displayed (0 = indefinitely)")]
        protected float duration;

        private MessageController displayedMessage;
        
        public void SetMessage(string message)
        {
            this.messageText = message;
        }

        public void SetMessage(UnityEngine.Object objMessage)
        {
            if(objMessage)
                this.messageText = objMessage.ToString();
        }

        public void AppendMessage(string message)
        {
            if (this.messageText.IsNullOrEmpty())
                SetMessage(message);
            else
                this.messageText += message;
        }

        public void AppendMessage(UnityEngine.Object objMessage)
        {
            if (objMessage)
            {
                if (this.messageText.IsNullOrEmpty())
                   SetMessage(objMessage.ToString());
                else
                    this.messageText += objMessage.ToString();
            }
        }


        public void HideMessage()
        {
            if (this.displayedMessage && this.displayedMessage.isActiveAndEnabled && this.displayedMessage.Duration <= 0)
                this.displayedMessage.Hide();
        }

        public void ShowMessage(UnityEngine.Object objMessage)
        {
            SetMessage(objMessage);
            ShowMessage();
        }

        public void ShowMessage()
        {
            if (!this.isActiveAndEnabled || this.messageText.IsNullOrEmpty())
                return;

            HideMessage();

            this.displayedMessage = DisplayMessage();
        }

        protected virtual MessageController DisplayMessage()
        {
            return MessagesManager.Instance.ShowStaticMessage(
                transform.position + this.messageOffset, 
                this.messageText, 
                this.messageGraphic, 
                this.duration);
        }

        void OnDisable()
        {
            HideMessage();
        }
    }
}