namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class MessageDisplayer : MonoBehaviour
    {

        [SerializeField]
        private string messageToDisplay;

        private MessageController displayedMessage;
        
        public void SetMessage(string message)
        {
            this.messageToDisplay = message;
        }

        public void HideMessage()
        {
            if (this.displayedMessage && this.displayedMessage.isActiveAndEnabled)
                this.displayedMessage.Hide();
        }

        public void ShowMessage()
        {
            if (!this.isActiveAndEnabled || this.messageToDisplay.IsNullOrEmpty())
                return;

            HideMessage();

            this.displayedMessage = MessagesManager.Instance.ShowStaticMessage(transform.position.WithY(1), this.messageToDisplay);
        }

        void OnDisable()
        {
            HideMessage();
        }
    }
}