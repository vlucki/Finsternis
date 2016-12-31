namespace Finsternis
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Extensions;

    public class MessageDisplayer : CustomBehaviour
    {

        [SerializeField]
        protected string messageText;

        [SerializeField]
        protected Sprite messageGraphic;

        [SerializeField]
        private Vector3 messageOffset = Vector3.up;

        [SerializeField]
        private bool randomizeOffset = false;

        [SerializeField][Tooltip("How long should this message be displayed (0 = indefinitely)")]
        protected float duration;

        protected MessageController displayedMessage;

        public Vector3 MessageOffset { get { return randomizeOffset ? Random.insideUnitSphere * (Random.value >= .5f ? 2 : -2): this.messageOffset; } }
        
        public void SetMessage(string message)
        {
            this.messageText = message;
        }

        public void SetMessage(UnityEngine.Object objMessage)
        {
            if(objMessage)
                this.messageText = objMessage.ToString();
        }

        public void LoadCardGraphic(Card card)
        {
            var sprites = Resources.LoadAll<Sprite>("Sprites/SPRITESHEET_cards");
            var chosen = sprites.Where(sprite => sprite.name.ToLower().Contains(card.MainName.ToString().ToLower())).ToList<Sprite>();
            if (!chosen.IsNullOrEmpty())
            {
                this.messageGraphic = chosen.GetRandom(Random.Range);
            }
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

        public void ShowMessage(string str)
        {
            SetMessage(str);
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