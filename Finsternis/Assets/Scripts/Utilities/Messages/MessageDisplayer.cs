namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityQuery;

    public class MessageDisplayer : MonoBehaviour
    {

        [SerializeField]
        protected string messageToDisplay;

        [SerializeField]
        protected Sprite messageGraphic;

        [SerializeField][Tooltip("How long should this message be displayed (0 = indefinitely)")]
        protected float duration;

        private MessageController displayedMessage;
        
        public void SetMessage(string message)
        {
            this.messageToDisplay = message;
        }

        public void SetMessage(UnityEngine.Object objMessage)
        {
            if(objMessage)
                this.messageToDisplay = objMessage.ToString();
        }


        public void HideMessage()
        {
            if (this.displayedMessage && this.displayedMessage.isActiveAndEnabled && this.displayedMessage.Duration <= 0)
                this.displayedMessage.Hide();
        }

        public void ShowMessage()
        {
            if (!this.isActiveAndEnabled || this.messageToDisplay.IsNullOrEmpty())
                return;

            HideMessage();

            this.displayedMessage = DisplayMessage();
        }

        protected virtual MessageController DisplayMessage()
        {
            return MessagesManager.Instance.ShowStaticMessage(
                transform.position.WithY(1), 
                this.messageToDisplay, 
                this.messageGraphic, 
                this.duration);
        }

        void OnDisable()
        {
            HideMessage();
        }
    }
}