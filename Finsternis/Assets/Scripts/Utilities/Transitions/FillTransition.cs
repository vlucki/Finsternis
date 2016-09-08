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
            while(Transitioning && Mathf.Abs(image.fillAmount - target) >= 0.05f)
            {
                image.fillAmount = Mathf.Lerp(image.fillAmount, target, interpolationAmount);
                yield return null;
            }

            image.fillAmount = target;
            End();
        }
    }
}