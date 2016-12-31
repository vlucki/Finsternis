namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using Extensions;
    using System;

    [RequireComponent(typeof(FadeInTransition), typeof(FadeOutTransition))]
    public class MessageController : CustomBehaviour
    {
        
        private Text messageField;
        private Image messageGraphic;

        private FadeInTransition fadeIn;
        private FadeOutTransition fadeOut;

        public float Duration { get; private set; }

        public string Message
        {
            get { return this.messageField.text; }
            set { this.messageField.text = value; }
        }

        public Sprite Graphic
        {
            get { return this.messageGraphic.sprite; }
            set { this.messageGraphic.sprite = value; }
        }

        protected virtual void Awake()
        {
            this.messageField = GetComponentInChildren<Text>();
            this.messageGraphic = GetComponentInChildren<Image>();
            this.fadeIn = GetComponent<FadeInTransition>();
            this.fadeOut = GetComponent<FadeOutTransition>();
        }

        public void Show(string message, Sprite graphic, float duration)
        {
            this.messageField.enabled = !message.IsNullOrEmpty();
            if (this.messageField.enabled)
                Message = message;

            this.messageGraphic.enabled = graphic != null;
            if (this.messageGraphic.enabled)
                Graphic = graphic;
            Show(duration);
        }

        public void Show(float duration)
        {
            this.Duration = duration;
            fadeIn.Begin();
            if (duration > 0)
                this.CallDelayed(fadeIn.Duration + duration, Hide);
        }

        public void Hide()
        {
            if (fadeOut.Transitioning)
            {
                fadeOut.Skip();
            }
            else
            {
                fadeIn.Skip();
                fadeOut.Begin();
            }
        }
    }
}