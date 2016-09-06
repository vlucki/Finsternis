namespace Finsternis
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityQuery;

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class FadeTransition : AnimationTransition
    {
        protected CanvasGroup canvasGroup;

        public enum FadeType { FadeIn = 0, FadeOut = 1}

        protected FadeType transitionType;

        [SerializeField]
        private bool presetAlpha = false;

        protected override void Awake()
        {
            this.canvasGroup = GetComponent<CanvasGroup>();

            if (presetAlpha)
                this.canvasGroup.alpha = (int)transitionType;
            trigger = transitionType.ToString();
            base.Awake();
        }
    }
}
