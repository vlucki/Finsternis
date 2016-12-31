namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using System;

    [RequireComponent(typeof(Image))]
    public abstract class FillTransition : Transition
    {

        public enum FillType { Fill = 1, ReverseFill = 0 }

        protected FillType transitionType;
        protected Image image;

        [SerializeField][Range(0.01f, 1)]
        private float interpolationAmount = 0.2f;

        [SerializeField]
        private bool presetFill = false;

        protected virtual void Awake()
        {
            this.image = GetComponent<Image>();
            if (presetFill)
                this.image.fillAmount = 1 - (int)transitionType;
            OnTransitionStarted.AddListener((t) => StartCoroutine(_DoFill()));
        }
        private IEnumerator _DoFill()
        {
            int target = (int)transitionType;

            while(Transitioning && Mathf.Abs(this.image.fillAmount - target) >= 0.05f)
            {
                this.image.fillAmount = Mathf.Lerp(this.image.fillAmount, target, this.interpolationAmount);
                yield return null;
            }

            this.image.fillAmount = target;
            End();
        }

        protected override void OnDisable()
        {
            image.fillAmount = (int)transitionType;

            base.OnDisable();
        }
    }
}